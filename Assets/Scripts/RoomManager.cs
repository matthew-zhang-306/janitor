using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
public class RoomManager : MonoBehaviour
{
    public GameObject room;
    public GameObject[] doors;
    public Grid levelGrid; 
    private int enemyCount;

    private UnityEvent roomClearEvent;

    private FloorController dirty;
    private FloorTilePopulator tiles;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var go in doors) {
            go.SetActive(false);
        }
        enemyCount = 0;
        
        if (roomClearEvent == null)
            roomClearEvent = new UnityEvent();

        roomClearEvent.AddListener(OnRoomClear);

        //Do something better here lol
        tiles = levelGrid.gameObject.transform.GetChild(0).GetComponent<FloorTilePopulator>();
        dirty = levelGrid.gameObject.transform.GetChild(1).GetComponent<FloorController>();
        //plan is, reuse Grid instance when entering rooms.
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            room.SetActive(true);

            int c = room.transform.childCount;
            for (int i = 0; i < c; i++) {
                var ec = room.transform.GetChild(i).GetComponent<EnemyController>();
                if (ec != null) {
                    enemyCount += 1;
                    ec.GetDeathEvent().AddListener(DecreaseEnemyCount);
                }
            }

            foreach (var go in doors) {
                go.SetActive(true);
            }
        }
        
        var col = this.GetComponent<Collider2D>();
        col.enabled = false;

    }

    public void SetRoom () {
        //make new room
    }

    private void DecreaseEnemyCount () {
        Debug.Log("dead here");
        enemyCount -= 1;
        if (enemyCount == 0) {
            roomClearEvent.Invoke();
        }
        else if (enemyCount < 0) {
            Debug.LogError("Oh no, why are there negative enemies?");
        }
    }

    private void OnRoomClear () {
        InvokeRepeating ("CheckPercent", 0f, 1f);
    }

    private void CheckPercent () {
        if (dirty.GetCleanPercent() > .80) {
            Debug.Log("hi there");
            foreach (var go in doors) {
                go.SetActive(false);
            }
            
            CancelInvoke();
            //Cancel enemy spawn here
        }
    }
}
