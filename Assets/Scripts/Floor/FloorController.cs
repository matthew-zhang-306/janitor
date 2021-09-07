using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
public class FloorMarkerData
{
    public FloorMarker floorMarker;
    public HashSet<Vector3Int> previousPositions;

    public FloorMarkerData(FloorMarker fm) {
        floorMarker = fm;
        previousPositions = new HashSet<Vector3Int>();
    }
}   


[RequireComponent(typeof(TilemapCollider2D))]
public class FloorController : MonoBehaviour
{
    [Tooltip("Use a BoxCollider2D to specify the area in which the dirty floor should spawn.")]
    public BoxCollider2D levelBounds;

    private Tilemap tm;
    private Tile[][] tiles;
    private Sprite[][] sprites;
    // public int maxTileHealth => tiles.Length - 1;
    public int maxTileHealth = 1;
    private Dictionary<Collider2D, FloorMarkerData> floorMarkers;

    private int currentFloorHealth;
    private int totalFloorHealth;

    private int sideLength;
    // Start is called before the first frame update
    void Start()
    {
        Sprite[] original = Resources.LoadAll<Sprite>("DirtyFloor/slime_floor_small");
        if (original.Length < 1) {
            Debug.LogError("Sprite Loading has failed for dirty floor");
            return;
        }
        sideLength = 6;

        // sprites = new Sprite[2][];
        // sprites[1] = original;
        //Load sprite
        //Lower number is lower health
        sprites = new Sprite[maxTileHealth + 1][];
        int size = original[0].texture.width;
        if (size != original[0].texture.height) {
            Debug.LogError("Sprite Not Square!");
            return;
        }

        sideLength = (int) Math.Sqrt (original.Length);

        sprites[0] = new Sprite[36];

        float pivot = original[0].pivot.x / (size / sideLength);
        
        for (int i = 1; i < maxTileHealth + 1; i++) {
            sprites[i] = new Sprite[original.Length];
            ////May need to look into removing these texture on destroy in case it sticks in memory for some reason
            Texture2D text = new Texture2D(size, size, TextureFormat.RGBA32, 1, true);
            
            // Graphics.CopyTexture(original[0].texture, text);
            var colors = original[0].texture.GetPixels();
            Debug.Log(colors.Length);
            for (int j = 0; j < colors.Length; j++) {
                colors[j].a = i * (1f / maxTileHealth);
            }
            text.SetPixels(colors);
            text.Apply(true);
            for (int j = 0; j < original.Length; j++) {
                sprites[i][j] = Sprite.Create(text, original[j].rect, new Vector2(pivot,pivot), 2 * size / sideLength);
                // sprites[i][j] = Instantiate<Sprite>(original[j]);
                
            }
            
        }
        
        // sprites[maxTileHealth] = original;
        //Sprite.Create()        
        

        tiles = new Tile[maxTileHealth + 1][];
        for (int health = 0; health < maxTileHealth + 1; health++) {
            Debug.Log(health);
            tiles[health] = new Tile[sprites[health].Length];
            for (int i = 0; i < sprites[health].Length; i++) {
                
                tiles[health][i] = ScriptableObject.CreateInstance<Tile>();
                // Sprite op_sprite = Instantiate<Sprite>(sprites[0]);
                
                tiles[health][i].sprite = sprites[health][i];
                tiles[health][i].name = "Dirty" + health.ToString();
                
            }
        }

        ////This is code for changing the texture itself. Do note that it will change the underlying textures unless there
        ////are some kind of resize algo
        // var test = sprites[3][0].texture;
        // var colors = test.GetPixels();
        // Debug.Log(colors.Length);
        // for (int i = 0; i < colors.Length; i++) {
        //     colors[i].a = 0.5f;
        // }
        // test.SetPixels(colors);
        // test.Apply(true);
        

        floorMarkers = new Dictionary<Collider2D, FloorMarkerData>();

        tm = this.GetComponent<Tilemap>();
        tm.ClearAllTiles();

        Vector3Int min = tm.WorldToCell(levelBounds.bounds.min);
        Vector3Int max = tm.WorldToCell(levelBounds.bounds.max);
        for (int x = min.x; x <= max.x; x++) {
            for (int y = min.y; y <= max.y; y++) {
                var cell = new Vector3Int(x, y, 0);

                // check if this is an open space
                if (Physics2D.OverlapPoint(tm.GetCellCenterWorld(cell), LayerMask.GetMask("Wall")) == null) {

                    tm.SetTile(cell, tiles[maxTileHealth][GetCoords(x,y)]);
                    totalFloorHealth += maxTileHealth;
                }
            }
        }

        currentFloorHealth = totalFloorHealth;
        levelBounds.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "FloorMarker") {
            FloorMarker floorMarker = col.GetComponent<FloorMarker>();
            if (floorMarker != null) {
                // track this object
                floorMarkers.Add(col, new FloorMarkerData(floorMarker));
            }
        }
    }

    void OnTriggerStay2D (Collider2D col) {
        if (floorMarkers.ContainsKey(col)) {
            HashSet<Vector3Int> cells = new HashSet<Vector3Int>();

            // loop over the collider's bounds to see what tiles it might be in
            Vector3Int min = tm.WorldToCell(col.bounds.min);
            Vector3Int max = tm.WorldToCell(col.bounds.max);
            for (int x = min.x; x <= max.x; x++) {
                for (int y = min.y; y <= max.y; y++) {
                    Vector3Int cell = new Vector3Int(x, y, 0);

                    // check if this position actually has a valid tile on it
                    var c_tile = tm.GetTile(cell);
                    if (c_tile == null || !c_tile.name.StartsWith("Dirty")) {
                        continue;
                    }

                    // check if the collider is actually touching this cell
                    var otherClosestPoint = col.ClosestPoint(tm.CellToWorld(cell));
                    Vector3Int otherClosestCell = tm.WorldToCell(otherClosestPoint);
                    if (cell != otherClosestCell) {
                        continue;
                    }

                    // this is a valid position that we should consider
                    cells.Add(cell);
                    if (floorMarkers[col].previousPositions.Contains(cell)) {
                        // the collider was on this tile before, so let's not change this tile again.
                        continue;
                    }

                    // deal "damage" to the floor
                    if (!int.TryParse(c_tile.name.Substring("Dirty".Length), out int tileHealth)) {
                        Debug.LogError("The floor tile " + c_tile.name + " does not look like 'DirtyX' where X is a number");
                        continue;
                    }
                    var oldTileHealth = tileHealth;
                    tileHealth -= floorMarkers[col].floorMarker.markAmount;
                    tileHealth = Mathf.Clamp(tileHealth, 0, maxTileHealth);

                    tm.SetTile(cell, tiles[tileHealth][GetCoords(x,y)]);
                    
                    currentFloorHealth += tileHealth - oldTileHealth;
                }
            }

            floorMarkers[col].previousPositions = cells;        
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        floorMarkers.Remove(other);
    }


    public float GetCleanPercent() {
        return 1f - (float)currentFloorHealth / (float)totalFloorHealth;
    }

    private int GetCoords (int x, int y) {
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
        return x_val + y_val * sideLength;
    }
}
