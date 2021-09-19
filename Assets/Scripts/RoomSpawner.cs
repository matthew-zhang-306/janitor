using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;


[RequireComponent(typeof(RoomManager))]
public class RoomSpawner : MonoBehaviour
{
    
    public int spawnAreaCount = 3;
    public float spawnTimer = 15f;
    private float m_spawnTimer;
    private RoomManager rm;

    public GameObject[] glist;

    public Tile tileToFlicker;
    private Tilemap overlay;

    void Start ()
    {
        m_spawnTimer = spawnTimer;
        rm = this.GetComponent<RoomManager>();
        var blank = new GameObject ("dirty spawn marker overlay");
        var generatedFloor = Instantiate (blank, rm.dirtyTiles.transform.parent);

        Destroy(blank);

        var tm = generatedFloor.AddComponent<Tilemap>();
        tm.tileAnchor = new Vector3(0.5f,0.5f,0);
        var tmr = generatedFloor.AddComponent<TilemapRenderer>();
        tmr.sortingOrder = 3;
        overlay = tm;
    }

    void FixedUpdate ()
    {
        m_spawnTimer -= Time.fixedDeltaTime;
        if (m_spawnTimer <= 0 && rm.IsRoomActive) {
            m_spawnTimer = spawnTimer;
            int retry = 5;
            for (int i = 0; i < spawnAreaCount - rm.NumEnemy; i++) {
                if (!AttemptSpawn () && retry > 0) {
                    i--;
                    retry -= 1;
                }
                
            }
        }
    }

    private bool AttemptSpawn ()
    {
        var min = rm.dirtyTiles.Min;
        var max = rm.dirtyTiles.Max;
        var rx = Random.Range(min.x, max.x);
        var ry = Random.Range(min.y, max.y);
        if (rm.dirtyTiles.IsTileDirty(new Vector2Int(rx,ry), .8f)) {
            int m = glist.Length;
            var g = glist[Random.Range(0, m)];

            var mark = new Marker (rx,ry, 5f, g, tileToFlicker);

            StartCoroutine(mark.Begin(overlay,rm));
                        
            return true;
            
        }
        else {
            return false;
        }
    }

    private class Marker 
    {
        public readonly int x;
        public readonly int y;
        public float m_timer;
        public readonly GameObject obj;
        public readonly Tile t;
        public Marker (int x, int y, float timer, GameObject obj, Tile tileToFlicker) 
        {
            this.x = x;
            this.y = y;
            this.m_timer = timer;
            this.obj = obj;
            this.t = tileToFlicker;
        }

        public IEnumerator Begin (Tilemap tm, RoomManager rm)
        {

            var count = 0;
            while (m_timer >= 0 && tm != null && rm.dirtyTiles.IsTileDirty(new Vector2Int(x,y), 0.2f)) {

                count ++;

                m_timer -= 0.5f;
                for (int xd = -2; xd <= 2; xd++) {
                    for (int yd = -2; yd <= 2; yd++) {
                        var loc = new Vector3Int (x + xd, y + yd, 0);
                        
                        tm.SetTile(loc, count % 2 == 0 ? t : null);
                    }
                }
                yield return new WaitForSeconds(0.5f);

            }
            for (int xd = -2; xd <= 2; xd++) {
                    for (int yd = -2; yd <= 2; yd++) {
                        var loc = new Vector3Int (x + xd, y + yd, 0);
                        
                        tm.SetTile(loc, null);
                    }
                }
            if (rm.dirtyTiles.IsTileDirty(new Vector2Int(x,y), 0.2f)) 
            {
                GameObject created = Instantiate(obj, tm.CellToWorld(new Vector3Int(x,y,0)), Quaternion.identity, rm.enemiesContainer);
                rm.InitEnemy(created.transform);
            }

            yield return 0;
        }
    }
}