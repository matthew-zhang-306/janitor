using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu]
public class DirtyTile : Tile
{
    [SerializeField] private int dirtyLevel = 0;
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        // this.sprite = null;
        base.RefreshTile(position, tilemap);
        for (int xd = -1; xd <= 1; xd++) {
            for (int yd = -1; yd <= 1; yd++) {
                if (yd == xd && xd == 0) {
                    continue;
                }
                Vector3Int location = new Vector3Int(position.x + xd, position.y + yd, position.z);
                if (IsLower (location, tilemap)) {
                    tilemap.RefreshTile(location);
                }
            }
        }
    }

    // public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    // {
    //     Texture2D red = new Texture2D (1, 1, TextureFormat.RGBA32, 1, true);
    //     red.SetPixel(0,0, Color.red);
    //     red.Apply(true);

    //     tileData.sprite = Sprite.Create(red, new Rect(0,0,1,1), Vector2.one / 2f, 2);
    //     // base.GetTileData(position, tilemap, ref tileData);
    // }

    private bool IsLower(Vector3Int pos, ITilemap tm) 
    {

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
