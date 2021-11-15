using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu]
public class DirtyTile : Tile
{
    [SerializeField] private int dirtyLevel = 0;

    [HideInInspector]
    public Sprite[,,,] sprites = new Sprite[4,4,4,4];

    private int dup;
    private int ddown;
    private int dleft;
    private int dright;

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        // this.sprite = null;
        base.RefreshTile(position, tilemap);

        //loop unrolled for optim
        

       
        var l1 = new Vector3Int(position.x + 1, position.y, position.z);
        var l2 = new Vector3Int(position.x - 1, position.y, position.z);
        var l3 = new Vector3Int(position.x, position.y + 1, position.z);
        var l4 = new Vector3Int(position.x, position.y - 1, position.z);
        RefreshIfIsLower (l1, tilemap);
        RefreshIfIsLower (l2, tilemap);
        RefreshIfIsLower (l3, tilemap);
        RefreshIfIsLower (l4, tilemap);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);

        var l1 = new Vector3Int(position.x + 1, position.y, position.z);
        var l2 = new Vector3Int(position.x - 1, position.y, position.z);
        var l3 = new Vector3Int(position.x, position.y + 1, position.z);
        var l4 = new Vector3Int(position.x, position.y - 1, position.z);

        
        dright = tilemap.GetTile<DirtyTile>(l1)?.dirtyLevel ?? 0;
        dleft = tilemap.GetTile<DirtyTile>(l2)?.dirtyLevel ?? 0;
        dup = tilemap.GetTile<DirtyTile>(l3)?.dirtyLevel ?? 0;
        ddown = tilemap.GetTile<DirtyTile>(l4)?.dirtyLevel ?? 0;

        
        tileData.sprite = sprites[dup,ddown,dright,dleft];

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
