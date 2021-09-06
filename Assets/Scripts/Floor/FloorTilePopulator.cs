using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
public class FloorTilePopulator : MonoBehaviour
{
    private Tile[] tiles;
    private Sprite[] sprites;
    private Tilemap tilemap;
    // Start is called before the first frame update
    void Start()
    {

        sprites = Resources.LoadAll<Sprite>("FloorTileSprites/FloorTileVersion2");
        if (sprites.Length < 1) {
            Debug.LogError("Not enough sprites to generate floor");
            return;
        }
        tiles = new Tile[sprites.Length];
        for (int i = 0; i < sprites.Length; i++) {
            //Apparently you need to create initializations here :/
            //good bye 4 hours of googling
            tiles[i] = ScriptableObject.CreateInstance<Tile>();
            tiles[i].sprite = sprites[i];
        }
        tilemap = this.GetComponent<Tilemap>();

        Vector3Int min = tilemap.cellBounds.min;
        Vector3Int max = tilemap.cellBounds.max;
        for (int x = min.x; x <= max.x; x++) {
            for (int y = min.y; y <= max.y; y++) {
                var cell = new Vector3Int(x, y, 0);

                //Alternate between light and dark tile

                var x_val = Math.Abs(x);
                var y_val = Math.Abs(y);

                //Center lined

                // var x_val = Math.Abs (x/2);
                // var y_val = Math.Abs (y/2);

                //Alternate between two chunk by two chunk tiles
                // var x_val = x/2;
                // var y_val = y/2;
                // if (x <= 0) {
                //     x_val = Math.Abs((x - 1)/2);
                // }
                // if (y <= 0) {
                //     y_val = Math.Abs((y - 1)/2);
                // }
                var alt = (x_val + y_val%2)%2;


                //currently assuming that first half of sprites is LIGHT
                //second half should be 'dirty'
                //We can also do other tiling later so it doesn't allows look like
                //a bathroom the entire time...

                // var xalt = (int) Math.Ceiling(Math.Sin(x * Math.PI) + 1);
                // xalt /= 2;

                // var yalt = (int) Math.Ceiling(Math.Sin(y * Math.PI) + 1);
                // yalt /= 2;

                tilemap.SetTile(cell, tiles[alt * (tiles.Length/2) + UnityEngine.Random.Range(0, tiles.Length/2)]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
