using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class RoomManager : MonoBehaviour
{
    [Header("Specify the boundary where the room camera will live. (White)")]
    public PolygonCollider2D roomCameraBounds;
    [Header("Specify the boundary where the room will be dirty. (Cyan)")]
    public BoxCollider2D roomFloorBounds;
    [Header("Specify the boundary where the player will trigger the doors to close. (Red)")]
    public BoxCollider2D roomTriggerBounds;
    private Hitbox roomTriggerHitbox;

    public Canvas roomUI;
    public Cinemachine.CinemachineVirtualCamera vcam;

    private bool roomActive;
    public bool IsRoomActive 
    {
        get => roomActive;
    }
    public GameObject room;
    public Transform doorsContainer;
    public Transform enemiesContainer;
    public Grid levelGrid; 
    private int enemyCount;
    
    public int NumEnemy
    {
        get => enemyCount;
    }
    private UnityEvent allEnemiesDefeatedEvent;

    public FloorController dirtyTiles;
    public Pathfinding pathfinding;
    private FloorTilePopulator tiles;

    public float roomClearThreshold;

    [Header("Put the Player into this")]
    public PlayerController player; // later we need to load this in some other way

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        enemyCount = 0;    
        if (allEnemiesDefeatedEvent == null)
            allEnemiesDefeatedEvent = new UnityEvent();

        roomTriggerHitbox = roomTriggerBounds.GetComponent<Hitbox>();

        // Eventually the plan will be to reuse Grid instances when entering rooms
        // Each level will not store either own dirtyTiles
        // tiles = levelGrid.gameObject.transform.GetChild(0).GetComponent<FloorTilePopulator>();
        // dirtyTiles = levelGrid.gameObject.transform.GetChild(1).GetComponent<FloorController>();
        InitializeRoom();
    }
    

    public void InitializeRoom () {
        // make new room
        foreach (Transform doorTransform in doorsContainer) {
            doorTransform.gameObject.SetActive(false);
        }

        foreach (Transform enemyTransform in enemiesContainer) {
            var ec = enemyTransform.GetComponent<BaseEnemy>();
            if (ec != null) {
                ec.player = player;
            }
        }

        dirtyTiles.InitializeFloor(roomFloorBounds.bounds);
        pathfinding.InitializePathfinding();

        roomUI.enabled = false;
        vcam.Priority = 0;
        vcam.Follow = player.cameraPos;
    }

    public void InitEnemy (Transform enemy) {

        var ec = enemy.GetComponent<BaseEnemy>();
        if (ec != null && ec.isActiveAndEnabled) {
            enemyCount += 1;

            ec.CanAct = true;
            ec.player = player;
            if (ec.navigator != null)
            {
                Debug.Log ("init enemy");

                ec.navigator.pathfinding = pathfinding;

            }
            
            ec.DeathEvent.AddListener(DecreaseEnemyCount);
        }
    }

    void FixedUpdate()
    {
        if (!roomActive && roomTriggerHitbox.IsColliding) {
            OnEnterRoom(roomTriggerHitbox.OtherCollider.GetComponent<PlayerController>());
            roomTriggerHitbox.enabled = false;
        }
        
        if (roomActive && enemyCount == 0 && dirtyTiles.GetCleanPercent() >= roomClearThreshold) {
            OnClearRoom();
            roomActive = false;
        }

    }
    
    private void OnEnterRoom(PlayerController player) {
        roomActive = true;
        room.SetActive(true);

        foreach (Transform enemyTransform in enemiesContainer) {
            InitEnemy (enemyTransform);
        }

        foreach (Transform doorTransform in doorsContainer) {
            doorTransform.gameObject.SetActive(true);
        }

        roomUI.enabled = true;
        vcam.Priority = 20;
    }

    private void OnClearRoom() {
        roomActive = false;

        foreach (Transform doorTransform in doorsContainer) {
            doorTransform.gameObject.SetActive(false);
        }

        roomUI.enabled = false;
        vcam.Priority = 0;

        //Cancel enemy spawn here
    }

    private void DecreaseEnemyCount () {
        enemyCount -= 1;
        if (enemyCount == 0) {
            allEnemiesDefeatedEvent.Invoke();
        }
        else if (enemyCount < 0) {
            Debug.LogError("Oh no, why are there negative enemies?");
        }
    }


    private void OnDrawGizmos() {
        if (roomCameraBounds != null) {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(roomCameraBounds.bounds.center, roomCameraBounds.bounds.size);
        }
        if (roomFloorBounds != null) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(roomFloorBounds.bounds.center, roomFloorBounds.bounds.size);
        }
        if (roomTriggerBounds != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(roomTriggerBounds.bounds.center, roomTriggerBounds.bounds.size);
        }
    }
}
