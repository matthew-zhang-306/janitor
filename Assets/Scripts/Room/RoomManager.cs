using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class RoomManager : MonoBehaviour
{
    public delegate void RoomDelegate(PlayerController pc, RoomManager rm);

    public enum RoomState {
        UNCLEARED, // room has not been entered yet
        ACTIVE, // room is currently in play
        CLEARED, // room has been completed but the player has not hit a checkpoint
        SAVED // room has been completed and player hit a checkpoint, progress is now permanent
    }

    private RoomState roomState = RoomState.UNCLEARED;
    public bool IsRoomActive => roomState == RoomState.ACTIVE;


    [Header("Specify the boundary where the room camera will live. (White)")]
    public PolygonCollider2D roomCameraBounds;
    [Header("Specify the boundary where the room will be dirty. (Cyan)")]
    public BoxCollider2D roomFloorBounds;
    [Header("Specify the boundary where the player will trigger the doors to close. (Red)")]
    public BoxCollider2D roomTriggerBounds;
    private Hitbox roomTriggerHitbox;

    public Canvas roomUI;
    public Cinemachine.CinemachineVirtualCamera vcam;

    private List<RoomComponentCopy> childrenCopy;
    private GameObject enemiesCopy;
    private FloorController.FloorData floorCopy;

    public GameObject[] RoomTurrets;
    public GameObject[] QuadTurret;
    public bool QuadTurretInRoom;
    

    public GameObject room;
    public Transform doorsContainer;
    public Transform enemiesContainer;
    public Grid levelGrid; 
    private int enemyCount;
    
    public int NumEnemy
    {
        get => enemyCount;
    }
    public UnityEvent AllEnemiesDefeatedEvent;
    public RoomDelegate onRoomClear;
    public static RoomDelegate OnEnter;
    public static RoomDelegate OnClear;

    public FloorController dirtyTiles;
    public Pathfinding pathfinding;
    private FloorTilePopulator tiles;

    [Range(0.5f, 0.95f)] public float roomClearThreshold;

    [Header("Put the Player into this")]
    public PlayerController player; // later we need to load this in some other way

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        enemyCount = 0;    
        if (AllEnemiesDefeatedEvent == null)
            AllEnemiesDefeatedEvent = new UnityEvent();

        roomTriggerHitbox = roomTriggerBounds.GetComponent<Hitbox>();


        childrenCopy = new List<RoomComponentCopy>();
        foreach (Transform child in this.transform) {
            var rcc = child.GetComponent<RoomComponentCopy>();
            if (rcc != null) {
                childrenCopy.Add (rcc);
            }
            
        }

        //Telling all turrets in array that they are room turrets
        Turret[] Turret = GameObject.FindObjectsOfType<Turret>();
        RoomTurrets = new GameObject[Turret.Length];

        for (int i = 0; i < Turret.Length; i++)
        {
            RoomTurrets[i] = Turret[i].gameObject;
        }

        foreach (GameObject controller in RoomTurrets)
        {
            controller.GetComponent<Turret>().RoomTurret = true;
        }

        QuadTurretFiring[] QuadTurret = GameObject.FindObjectsOfType<QuadTurretFiring>();
        RoomTurrets = new GameObject[QuadTurret.Length];

        for (int i = 0; i < QuadTurret.Length; i++)
        {
            RoomTurrets[i] = QuadTurret[i].gameObject;
        }

        foreach (GameObject controller in RoomTurrets)
        {
            controller.GetComponent<QuadTurretFiring>().RoomTurret = true;
        }

        // Eventually the plan will be to reuse Grid instances when entering rooms
        // Each level will not store either own dirtyTiles
        // tiles = levelGrid.gameObject.transform.GetChild(0).GetComponent<FloorTilePopulator>();
        // dirtyTiles = levelGrid.gameObject.transform.GetChild(1).GetComponent<FloorController>();
        InitializeRoom();
    }
    

    public void InitializeRoom () {
        // make new room
        OpenDoors();

        foreach (Transform enemyTransform in enemiesContainer) {
            var ec = enemyTransform.GetComponent<BaseEnemy>();

            if (ec != null) {
                ec.player = player;
            }
        }

        dirtyTiles.InitializeFloor(roomFloorBounds);
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
                ec.navigator.pathfinding = pathfinding;
            }
            
            ec.onDeath += DecreaseEnemyCount;
        }
    }

    void FixedUpdate()
    {
        if (roomState == RoomState.UNCLEARED && roomTriggerHitbox.IsColliding && roomTriggerHitbox.enabled) {
            Debug.Log ("hi there I will now start the room");
            OnEnterRoom(roomTriggerHitbox.OtherCollider.GetComponent<PlayerController>());
            roomTriggerHitbox.enabled = false;
        }
        
        if (roomState == RoomState.ACTIVE && enemyCount == 0 && dirtyTiles.GetCleanPercent() >= roomClearThreshold) {
            Debug.Log ("room finished");
            OnClearRoom(true);
        }

        if (roomState == RoomState.ACTIVE && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.L)) {
            ForceClearRoom();
        }
    }
    
    private void OnEnterRoom(PlayerController player) {
        PlayerController.OnRestart += ResetRoom;

        //Copy room for restart
        //should NOT include initial enemies due to binds and such
        foreach (RoomComponentCopy child in childrenCopy)
        {
            child?.CreateCopy();
        }
        floorCopy = dirtyTiles.SaveFloor();

        OnEnter?.Invoke(player, this);

        roomState = RoomState.ACTIVE;
        room.SetActive(true);

        //Activate the Turrets
        StartTurrets();

        enemiesCopy = Instantiate (enemiesContainer.gameObject, enemiesContainer.parent);
        enemiesCopy.SetActive(false);

        // handle enemies
        foreach (Transform enemyTransform in enemiesContainer) {
            InitEnemy (enemyTransform);
        }

        CloseDoors();

        roomUI.enabled = true;
        vcam.Priority = 20;
    }

    private void ResetRoom (PlayerController player) {
        PlayerController.OnHitCheckpoint -= SaveRoom;

        for (int i = 0; i < childrenCopy.Count; i++) {
            childrenCopy[i] = childrenCopy[i]?.Replace();
        }
        OnClearRoom(false);

        roomState = RoomState.UNCLEARED;
        dirtyTiles.SetFloor (floorCopy);

        Destroy (enemiesContainer.gameObject);
        enemiesContainer = enemiesCopy.transform;
        enemiesContainer.gameObject.SetActive(true);
        roomTriggerHitbox.enabled = true;
    }

    private void OnClearRoom(bool save) {
        if (save) {
            roomState = RoomState.CLEARED;
            PlayerController.OnHitCheckpoint += SaveRoom;

            onRoomClear?.Invoke(player, this);
        }

        StopTurrets();
        OpenDoors();

        roomUI.enabled = false;
        vcam.Priority = 0;
        enemyCount = 0;

        
        OnClear?.Invoke(player, this);
        // InteractableSpawner.i.SpawnItem("Health Pickup", player.transform.position);
        
        //Cancel enemy spawn here
    }

    private void ForceClearRoom() {
        foreach (Transform enemyT in enemiesContainer) {
            Destroy(enemyT.gameObject);
        }
        enemyCount = 0;
        OnClearRoom(true);
    }

    private void SaveRoom(PlayerController _) {
        roomState = RoomState.SAVED;
        PlayerController.OnRestart -= ResetRoom;
        PlayerController.OnHitCheckpoint -= SaveRoom;
    }

    private void DecreaseEnemyCount (BaseEnemy _) {
        enemyCount -= 1;
        if (enemyCount == 0) {
            AllEnemiesDefeatedEvent.Invoke();
        }
        else if (enemyCount < 0) {
            Debug.LogError("Oh no, why are there negative enemies?");
        }
    }


    public void OpenDoors() {
        foreach (Transform doorTransform in doorsContainer) {
            Door door = doorTransform.GetComponent<Door>();
            if (door != null) {
                door.OpenDoor();
            }
            else {
                // legacy system: deactivate the door
                doorTransform.gameObject.SetActive(false);
            }
        }
    }

    public void CloseDoors() {
        foreach (Transform doorTransform in doorsContainer) {
            Door door = doorTransform.GetComponent<Door>();
            if (door != null) {
                door.CloseDoor();
            }
            else {
                // legacy system: activate the door
                doorTransform.gameObject.SetActive(true);
            }
        }
    }

    public void StartTurrets()
    {
        //Activate the Turrets
        Turret[] Turret = GameObject.FindObjectsOfType<Turret>();
        RoomTurrets = new GameObject[Turret.Length];

        for (int i = 0; i < Turret.Length; i++)
        {
            RoomTurrets[i] = Turret[i].gameObject;
        }

        foreach (GameObject controller in RoomTurrets)
        {
            controller.GetComponent<Turret>().RoomActivated = true;
        }

        QuadTurretFiring[] QuadTurret = GameObject.FindObjectsOfType<QuadTurretFiring>();
        RoomTurrets = new GameObject[QuadTurret.Length];

        for (int i = 0; i < QuadTurret.Length; i++)
        {
            RoomTurrets[i] = QuadTurret[i].gameObject;
        }

        foreach (GameObject controller in RoomTurrets)
        {
            controller.GetComponent<QuadTurretFiring>().RoomActivated = true;
        }
    }

    public void StopTurrets()
    {
        //Activate the Turrets
        Turret []
        Turret = GameObject.FindObjectsOfType<Turret>();
        RoomTurrets = new GameObject[Turret.Length];

        for (int i = 0; i<Turret.Length; i++)
        {
            RoomTurrets[i] = Turret[i].gameObject;
        }

        foreach (GameObject controller in RoomTurrets)
        {
            controller.GetComponent<Turret>().RoomActivated = false;
        }

        QuadTurretFiring[] QuadTurret = GameObject.FindObjectsOfType<QuadTurretFiring>();
        RoomTurrets = new GameObject[QuadTurret.Length];

        for (int i = 0; i < QuadTurret.Length; i++)
        {
            RoomTurrets[i] = QuadTurret[i].gameObject;
        }

        foreach (GameObject controller in RoomTurrets)
        {
            controller.GetComponent<QuadTurretFiring>().RoomActivated = false;
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




    