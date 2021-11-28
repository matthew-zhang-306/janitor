using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu]
public class DirtyTile : Tile
{
    private TilemapRenderer tmr;

    [SerializeField] private int dirtyLevel = 0;

    [HideInInspector]
    public Sprite[,,,] sprites = new Sprite[4,4,4,4];

    private int[,] dvals = new int[3,3];

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        // this.sprite = null;
        base.RefreshTile(position, tilemap);

        //loop unrolled for optim
       
        // var l1 = new Vector3Int(position.x + 1, position.y, position.z);
        // var l2 = new Vector3Int(position.x - 1, position.y, position.z);
        // var l3 = new Vector3Int(position.x, position.y + 1, position.z);
        // var l4 = new Vector3Int(position.x, position.y - 1, position.z);
        // RefreshIfIsLower (l1, tilemap);
        // RefreshIfIsLower (l2, tilemap);
        // RefreshIfIsLower (l3, tilemap);
        // RefreshIfIsLower (l4, tilemap);
        // dvals = new int[3,3];
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                RefreshIfIsLower (new Vector3Int(position.x + x, position.y+y, position.z), tilemap);
                
            }
        }
        
        
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                dvals[x+1,y+1] = tilemap.GetTile<DirtyTile>(new Vector3Int(position.x + x, position.y+y, position.z))?.dirtyLevel ?? 0;
            }
        }
        // tileData.color = new Color32 ((byte)57,0,0,0);
        // tileData.color = new Color32((byte)((dvals[0,0] << 6) + (dvals[0,1] << 4) + (dvals[0,2] << 2) + (dvals[1,0])), 
        //                              (byte)((dvals[1,1] << 6) + (dvals[1,2] << 4) + (dvals[2,0] << 2) + (dvals[2,1])), 
        //                              (byte) dvals[2,2], 0);
        if (dvals[0,0] > 3) {
            Debug.Log(dvals[0,0] << 4 + dvals[0,1]);
        }
        
        tileData.color = new Color32 ((byte)((dvals[0,0] << 4) + dvals[0,1]), 
                                     (byte)((dvals[0,2] << 4) + dvals[1,0]),
                                     (byte)((dvals[1,2] << 4) + dvals[2,0]),
                                     (byte)((dvals[2,1] << 4) + dvals[2,2]));
        
    }

    private bool RefreshIfIsLower(Vector3Int pos, ITilemap tm) 
    {
        var t = tm.GetTile<DirtyTile>(pos);
        if ((t?.dirtyLevel ?? 100) != this.dirtyLevel) {
            tm.RefreshTile(pos);
        }
        return true;
    }
    
    public void SetDirty (int val)
    {
        if (val < 0) {
            Debug.LogError("Dirty value cannot be less than 0!");
        }
        dirtyLevel = val;
    }

    public int GetDirty () 
    {
        return dirtyLevel;        
    }
}
