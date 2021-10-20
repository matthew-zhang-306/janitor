using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
public class FloorTilePopulator : MonoBehaviour
{
    private List<Tile[]> tileList;
    private List<int> lengths;
    private List<Sprite[]> spriteList;
    private Tilemap tilemap;
    [SerializeField] private bool generateOnRuntime = true;

    [Header("Set min and max to both 0 if you want original behavior")]
    [SerializeField] private Vector3Int min;
    [SerializeField] private Vector3Int max;
    // Start is called before the first frame update
    void Start()
    {
        if (min == max) {
            min = tilemap.cellBounds.min;
            max = tilemap.cellBounds.max;
        }
        if (generateOnRuntime) Load();
        
    }

    public void Load ()
    {
        spriteList = new List<Sprite[]>();
        tileList = new List<Tile[]>();
        lengths = new List<int>();
        spriteList.Add (Resources.LoadAll<Sprite>("FloorTileSprites/floorvar_2"));

        foreach (var sprites in spriteList) {
                if (sprites.Length < 1) {
                Debug.LogError("Not enough sprites to generate floor");
                return;
            }
            var tiles = new Tile[sprites.Length];
            int sideLength = (int) Mathf.Sqrt (sprites.Length);
            for (int i = 0; i < sprites.Length; i++) {
                //Apparently you need to create initializations here :/
                //good bye 4 hours of googling
                tiles[i] = ScriptableObject.CreateInstance<Tile>();
                tiles[i].sprite = sprites[i];
            }
            tilemap = this.GetComponent<Tilemap>();
            tileList.Add (tiles);
            lengths.Add (sideLength);
        }
        for (int x = min.x; x <= max.x; x++) {
            for (int y = min.y; y <= max.y; y++) {
                var cell = new Vector3Int(x, y, 0);

                // tilemap.SetTile(cell, tiles[alt * (tiles.Length/2) + UnityEngine.Random.Range(0, tiles.Length/2)]);

                tilemap.SetTile(cell, tileList[0][GetCoords(x,y, lengths[0])]);
            }
        }
    }

    public void Clear()
    {
        tilemap.ClearAllTiles();
    }

    private int GetCoords (int x, int y, int sideLength) {
        var x_val = x % sideLength;
        var y_val = y % sideLength;

        
        if (x_val < 0) {
            x_val += sideLength;
        }
        if (y_val < 0) {
            y_val += sideLength;
        }

        //flip y
        y_val = sideLength - 1 - y_val;
        //return a index from an array size sideLength^2
        return x_val + y_val * sideLength;
    }
}
