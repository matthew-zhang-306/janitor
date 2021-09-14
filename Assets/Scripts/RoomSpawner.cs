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
    void Start ()
    {
        m_spawnTimer = spawnTimer;
        rm = this.GetComponent<RoomManager>();
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
            var tm = rm.dirtyTiles.GetComponent<Tilemap>();
            GameObject created = Instantiate(glist[Random.Range(0, m)], tm.CellToWorld(new Vector3Int(rx,ry,0)), Quaternion.identity, rm.enemiesContainer);

            //var ec = created.GetComponent<BaseEnemy>();

            rm.InitEnemy(created.transform);
            //Do init stuff here for created enemy
            
            return true;
            
        }
        else {
            return false;
        }
    }
}