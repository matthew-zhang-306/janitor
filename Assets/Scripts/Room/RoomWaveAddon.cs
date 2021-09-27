using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

[RequireComponent(typeof(RoomManager))]
[RequireComponent(typeof(RoomSpawnerAddon))]
public class RoomWaveAddon : MonoBehaviour
{
    // room manager, lazily initialized so that it is available at editor time
    private RoomManager _roomManager;
    public RoomManager roomManager => _roomManager ?? (_roomManager = GetComponent<RoomManager>());

    private RoomSpawnerAddon rs;
    public TextAsset jsonFile;
    public EnemyTypesSO enemyTypesSO;
    private Dictionary<string, EnemyTypesSO.EnemyType> enemyTypes;

    private Waves waves;
    void Start()
    {
        rs = this.GetComponent<RoomSpawnerAddon>();
        waves = JsonUtility.FromJson<Waves>(jsonFile.text);
        enemyTypes = enemyTypesSO.GetEnemyTypes();

        StartCoroutine (WaitForRoom ());
        roomManager.onRoomClear += () => {
            StopAllCoroutines();
            StartCoroutine (WaitForRoom ());
        };
    }

    public IEnumerator WaitForRoom ()
    {
        yield return new WaitUntil (() => roomManager.IsRoomActive);

        var tm = roomManager.dirtyTiles.gameObject.GetComponent<Tilemap>();
        foreach (var wave in waves.waves) {
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
            waveSpawn.GetPosition(tm),
            Quaternion.identity,
            roomManager.enemiesContainer
        );
        roomManager.InitEnemy(created.transform);
        
        yield return 0;
    }
    
    [System.Serializable]
    public class Waves
    {
        public Wave[] waves;
    }

    [System.Serializable]
    public class Wave
    {
        public float thresh;
        public WaveSpawn[] spawns;
    }

    [System.Serializable]
    public class WaveSpawn
    {
        public float xcoord;
        public float ycoord;
        public string enemy;
        public float delay;

        public Vector3 GetPosition(Tilemap tm) {
            // lerp between floored and ceiled tile positions
            var lo = tm.GetCellCenterWorld(new Vector3Int(Mathf.FloorToInt(xcoord), Mathf.FloorToInt(ycoord), 0));
            var hi = tm.GetCellCenterWorld(new Vector3Int(Mathf.CeilToInt(xcoord), Mathf.CeilToInt(ycoord), 0));
            return new Vector3(Mathf.Lerp(lo.x, hi.x, xcoord.Mod(1)), Mathf.Lerp(lo.y, hi.y, ycoord.Mod(1)));
        }

        public void SetPosition(Vector3 worldPos, Tilemap tm) {
            var cell = tm.WorldToCell(worldPos);
            var cellWorld = tm.GetCellCenterWorld(cell);

            // get the cell size in the hackiest way possible
            var cellSize = tm.GetCellCenterWorld(new Vector3Int(cell.x + 1, cell.y + 1, 0)) - cellWorld;

            xcoord = (float)cell.x + (worldPos.x - cellWorld.x) % cellSize.x;
            ycoord = (float)cell.y + (worldPos.y - cellWorld.y) % cellSize.y;
        }
    }
}