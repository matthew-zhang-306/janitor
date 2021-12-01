using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;


[RequireComponent(typeof(RoomManager))]
public class RoomSpawnerAddon : MonoBehaviour
{
    // public AudioSource spawnSound;
    public int spawnAreaCount = 3;
    public float spawnTimer = 15f;
    private float m_spawnTimer;
    private RoomManager rm;

    public GameObject[] glist;

    private Tilemap overlay;

    public GameObject spawnMarkerPrefab;

    void Start ()
    {
        m_spawnTimer = spawnTimer;
        rm = this.GetComponent<RoomManager>();

        rm.onRoomClear += (a, b) => {
            StopAllCoroutines();
        };

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
            var enemy = glist[Random.Range(0, m)];

            var mark = new Marker (rx,ry, 5f, enemy);
           
            StartCoroutine(mark.Begin(rm.dirtyTiles.GetComponent<Tilemap>(), rm));
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
        public AudioSource sp;
        // private RoomSpawnerAddon ad = new RoomSpawnerAddon();

        //Time from marker creation to spawning enemy
        public float m_timer;

        public readonly GameObject enemy;
        
        public Marker (int x, int y, float timer, GameObject obj) 
        {
            this.x = x;
            this.y = y;
            this.m_timer = timer;
            this.enemy = obj;
        }

        public IEnumerator Begin (Tilemap tm, RoomManager rm)
        {

            var count = 0;
            while (m_timer >= 0 && tm != null && rm.dirtyTiles.IsTileDirty(new Vector2Int(x,y), 0.2f)) {

                count ++;

                m_timer -= 0.5f;
                
                
                yield return new WaitForSeconds(0.5f);

            }
            
            if (rm.dirtyTiles.IsTileDirty(new Vector2Int(x,y), 0.2f)) 
            {
                // var rmnumber = GameObject.Find("Room (7)");
                // sp = rmnumber.GetComponent<RoomSpawnerAddon>().spawnSound;
                //  sp.Play();

                yield return new WaitForSeconds(0.5f);

                GameObject created = Instantiate(enemy, tm.CellToWorld(new Vector3Int(x,y,0)), Quaternion.identity, rm.enemiesContainer);
                rm.InitEnemy(created.transform);
            }

            yield return 0;
        }
    }
}