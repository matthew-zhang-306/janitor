using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

[RequireComponent(typeof(RoomManager))]
public class RoomWaveAddon : MonoBehaviour
{
    // room manager, lazily initialized so that it is available at editor time
    private RoomManager _roomManager;
    public RoomManager roomManager => _roomManager ?? (_roomManager = GetComponent<RoomManager>());

    public EnemyTypesSO enemyTypesSO;
    private Dictionary<string, EnemyTypesSO.EnemyType> enemyTypes;

    [SerializeField] private Wave[] waves;

    void Start()
    {
        enemyTypes = enemyTypesSO.GetEnemyTypes();

        StartCoroutine (WaitForRoom ());
        roomManager.onRoomClear += (a, b) => {
            StopAllCoroutines();
            StartCoroutine (WaitForRoom ());
        };
    }

    public IEnumerator WaitForRoom ()
    {
        yield return new WaitUntil (() => roomManager.IsRoomActive);

        var tm = roomManager.dirtyTiles.gameObject.GetComponent<Tilemap>();
        foreach (var wave in waves) {
            foreach (var waveSpawn in wave.spawns) {
                StartCoroutine(SetWaveSpawn(waveSpawn, wave.thresh, tm));
            }
        }
        
    }

    public IEnumerator SetWaveSpawn (WaveSpawn waveSpawn, float thresh, Tilemap tm)
    {
        if (!enemyTypes.ContainsKey(waveSpawn.enemy)) {
            Debug.LogError("Room " + gameObject + " wants to spawn a \"" + waveSpawn.enemy + "\" enemy, but there is no such enemy!");
            yield break;
        }

        yield return new WaitUntil(() => roomManager.dirtyTiles.GetCleanPercent() >= thresh);
        
        GameObject created = Instantiate(
            enemyTypes[waveSpawn.enemy].prefab,
            WaveSpawn.XYToPosition(waveSpawn.xcoord, waveSpawn.ycoord, tm),
            Quaternion.identity,
            roomManager.enemiesContainer
        );
        roomManager.InitEnemy(created.transform);
        
        yield return 0;
    }
    
    [System.Serializable]
    public struct WaveArray
    {
        public Wave[] waves;
        public WaveArray(Wave[] w) {
            waves = w;
        }
    }

    [System.Serializable]
    public struct Wave
    {
        public float thresh;
        public WaveSpawn[] spawns;
    }

    [System.Serializable]
    public struct WaveSpawn
    {
        public float xcoord;
        public float ycoord;
        public string enemy;
        public float delay;

        public static Vector3 XYToPosition(float xcoord, float ycoord, Tilemap tm) {
            // lerp between floored and ceiled tile positions
            var lo = tm.GetCellCenterWorld(new Vector3Int(Mathf.FloorToInt(xcoord), Mathf.FloorToInt(ycoord), 0));
            var hi = tm.GetCellCenterWorld(new Vector3Int(Mathf.CeilToInt(xcoord), Mathf.CeilToInt(ycoord), 0));
            return new Vector3(Mathf.Lerp(lo.x, hi.x, xcoord.Mod(1)), Mathf.Lerp(lo.y, hi.y, ycoord.Mod(1)));
        }

        public static float PositionToX(Vector3 worldPos, Tilemap tm) {
            var cell = tm.WorldToCell(worldPos);
            var cellWorld = tm.GetCellCenterWorld(cell);

            // get the cell size in the hackiest way possible
            var cellSize = tm.GetCellCenterWorld(new Vector3Int(cell.x + 1, cell.y + 1, 0)) - cellWorld;
            
            return (float)cell.x + (worldPos.x - cellWorld.x) % cellSize.x;
        }

        public static float PositionToY(Vector3 worldPos, Tilemap tm) {
            var cell = tm.WorldToCell(worldPos);
            var cellWorld = tm.GetCellCenterWorld(cell);

            // get the cell size in the hackiest way possible
            var cellSize = tm.GetCellCenterWorld(new Vector3Int(cell.x + 1, cell.y + 1, 0)) - cellWorld;

            return (float)cell.y + (worldPos.y - cellWorld.y) % cellSize.y;
        }
    }
}