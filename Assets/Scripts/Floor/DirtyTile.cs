using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DirtyTile : Tile
{
    private Texture2D text = null;
    private double baseOpacity = 1f;
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

    private bool IsLower(Vector3Int pos, ITilemap tm) 
    {

        return false;
    }
    
}
