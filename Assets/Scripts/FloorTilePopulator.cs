using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
public class FloorTilePopulator : MonoBehaviour
{
    public Tile[] tiles;
    private Tilemap tilemap;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = this.GetComponent<Tilemap>();
        Debug.Log(tiles.Length);
        Vector3Int min = tilemap.cellBounds.min;
        Vector3Int max = tilemap.cellBounds.max;
        for (int x = min.x; x <= max.x; x++) {
            for (int y = min.y; y <= max.y; y++) {
                var cell = new Vector3Int(x, y, 0);
                Debug.Log((Math.Abs (x) + Math.Abs(y)%2)%4);
                tilemap.SetTile(cell, tiles[(Math.Abs (x) + Math.Abs(y)%2)%4]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
