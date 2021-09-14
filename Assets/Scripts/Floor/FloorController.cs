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


[RequireComponent(typeof(BoxCollider2D))]
public class FloorController : MonoBehaviour
{
    [Tooltip("Use a BoxCollider2D to specify the area in which the dirty floor should spawn.")]
    public BoxCollider2D levelBounds;

    private Tilemap tm;
    private static DirtyTile[][] tiles = null;
    private static Sprite[][] sprites = null;
    // public int maxTileHealth => tiles.Length - 1;
    public int maxTileHealth = 1;
    private Dictionary<Collider2D, FloorMarkerData> floorMarkers;

    private int currentFloorHealth;
    private int totalFloorHealth;

    private int sideLength;

    private Vector3Int _min;
    public Vector3Int Min
    {
        get => _min;
    }
    private Vector3Int _max;
    public Vector3Int Max
    {
        get => _max;
    }
    // Start is called before the first frame update
    void Awake()
    {

        if (tiles == null || sprites == null) {
            //Requires a sliced sprite map for this to work.
            Sprite[] original = Resources.LoadAll<Sprite>("DirtyFloor/slime_floor_continuous");
            if (original.Length < 1) {
                Debug.LogError("Sprite Loading has failed for dirty floor");
                return;
            }
            sideLength = 6;

            
            //Load sprite
            //Lower number is lower health
            sprites = new Sprite[maxTileHealth + 1][];

            //Size refers the the original texture size (non cut)
            int size = original[0].texture.width;
            if (size != original[0].texture.height) {
                Debug.LogError("Sprite Not Square!");
                return;
            }

            sideLength = (int) Math.Sqrt (original.Length);

            if (original.Length - sideLength * sideLength != 0) {
                Debug.LogError("Sprite sheet slice went wrong");
            }

            //honestly I have no clue how pivot works but eh I think this does it
            float pivot = original[0].pivot.x / (size / sideLength);
            
            for (int i = 0; i < maxTileHealth + 1; i++) {
                sprites[i] = new Sprite[original.Length];
                ////May need to look into removing these texture on destroy in case it sticks in memory for some reason
                Texture2D text = new Texture2D(size, size, TextureFormat.RGBA32, 1, true);
                
                var colors = original[0].texture.GetPixels();
                for (int j = 0; j < colors.Length; j++) {
                    colors[j].a = i * (1f / maxTileHealth) * 0.65f;
                }
                text.SetPixels(colors);
                text.Apply(true);

                //Create sprites sheet for each layer here. 
                for (int j = 0; j < original.Length; j++) {
                    sprites[i][j] = Sprite.Create(text, original[j].rect, new Vector2(pivot,pivot), 2 * size / sideLength);
                }
                
            }

            tiles = new DirtyTile[maxTileHealth + 1][];
            for (int health = 0; health < maxTileHealth + 1; health++) {
                tiles[health] = new DirtyTile[sprites[health].Length];

                //Tile create per sprite.
                //Separate from sprite creation because importing tiles is too tedious
                for (int i = 0; i < sprites[health].Length; i++) {
                    
                    tiles[health][i] = ScriptableObject.CreateInstance<DirtyTile>();                
                    tiles[health][i].sprite = sprites[health][i];
                    tiles[health][i].SetDirty(health);
                    //we probs shouldn't base the dirtyness of a tile by its name...
                    tiles[health][i].name = "Dirty" + health.ToString();
                    
                }
            }
        }
        

        floorMarkers = new Dictionary<Collider2D, FloorMarkerData>();

    }

    public void InitializeFloor(Bounds levelBounds) {
        tm = this.GetComponent<Tilemap>();
        tm.ClearAllTiles();

        _min = tm.WorldToCell(levelBounds.min);
        _max = tm.WorldToCell(levelBounds.max);
        for (int x = _min.x; x <= _max.x; x++) {
            for (int y = _min.y; y <= _max.y; y++) {
                var cell = new Vector3Int(x, y, 0);

                // check if this is an open space
                if (Physics2D.OverlapPoint(tm.GetCellCenterWorld(cell), LayerMask.GetMask("Wall")) == null) {
                    tm.SetTile(cell, tiles[maxTileHealth][GetCoords(x,y)]);
                    totalFloorHealth += maxTileHealth;
                }
            }
        }

        currentFloorHealth = totalFloorHealth;
        var col = GetComponent<BoxCollider2D>();
        col.size = levelBounds.size;
        // col.offset = levelBounds.center;

    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (tm == null) {
            // floor has not been initialized, ignore collisions
            return;
        }

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

            int changed = 0;
            for (int x = min.x; x <= max.x; x++) {
                for (int y = min.y; y <= max.y; y++) {
                    Vector3Int cell = new Vector3Int(x, y, 0);

                    // check if this position actually has a valid tile on it
                    var c_tile = tm.GetTile(cell);
                    if (c_tile == null || c_tile.GetType() != typeof (DirtyTile)) {
                        continue;
                    }
                    DirtyTile d_tile = (DirtyTile) c_tile;
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
                    // if (!int.TryParse(c_tile.name.Substring("Dirty".Length), out int tileHealth)) {
                    //     Debug.LogError("The floor tile " + d_tile.name + " does not look like 'DirtyX' where X is a number");
                    //     continue;
                    // }
                    int tileHealth = d_tile.GetDirty();
                    var oldTileHealth = tileHealth;
                    tileHealth -= floorMarkers[col].floorMarker.markAmount;
                    tileHealth = Mathf.Clamp(tileHealth, 0, maxTileHealth);

                    tm.SetTile(cell, tiles[tileHealth][GetCoords(x,y)]);
                    changed += oldTileHealth - tileHealth;
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
        return 1f - currentFloorHealth / (totalFloorHealth + (float) 1e-6);
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

    public bool IsTileDirty (Vector2Int cell, float threshold)
    {
        //Look in a 5x5 box around called tile and check how 'dirty' it is
        int total = 0;
        for (int x = -2; x <= 2; x++) {
            for (int y = -2; y <= 2; y++) {
                var loc = new Vector3Int (cell.x + x, cell.y + y, 0);
                var tile = tm.GetTile<DirtyTile>(loc);
                if (tile != null) {
                    total += tile.GetDirty();
                    
                }
                else {
                    //penalty for being a non tile
                    total -= 1;
                }
            }
        }

        //If area around cell is somewhat dirty
        if (total / (maxTileHealth * 25f) >= threshold) {
            return true;
        }
        return false;
    }
}

