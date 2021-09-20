using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

[RequireComponent(typeof(RoomManager))]
[RequireComponent(typeof(RoomSpawnerAddon))]
public class RoomWaveAddon : MonoBehaviour
{
    private RoomManager rm;
    private RoomSpawnerAddon rs;
    public TextAsset jsonFile;

    private Waves waves;
    void Start()
    {
        rm = this.GetComponent<RoomManager>();
        rs = this.GetComponent<RoomSpawnerAddon>();
        waves = JsonUtility.FromJson<Waves>(jsonFile.text);
        StartCoroutine (WaitForRoom ());
    }

    public IEnumerator WaitForRoom ()
    {
        yield return new WaitUntil (() => rm.IsRoomActive);
        Debug.Log("Placing Wave Spawns");
        var tm = rm.dirtyTiles.gameObject.GetComponent<Tilemap>();
        foreach (var w in waves.waves) {
            StartCoroutine (SetWave(w, tm));
        }
    }

    public IEnumerator SetWave (WaveNode w, Tilemap tm)
    {
        if (w.index >= rs.glist.Length) {
            Debug.LogError("Index out of bounds for wave enemy");
            yield return 0;
        }
        yield return new WaitUntil (() => rm.dirtyTiles.GetCleanPercent() >= w.thresh);
        Debug.Log("Triggering Wave Spawn");

        var worldPos = tm.CellToWorld (new Vector3Int (w.xcoord, w.ycoord, 0 ));
        
        GameObject created = Instantiate(rs.glist[w.index], worldPos, Quaternion.identity, rm.enemiesContainer);
        rm.InitEnemy(created.transform);
        yield return 0;
    }
    
    [System.Serializable]
    public class Waves
    {
        public WaveNode[] waves;

    }

    [System.Serializable]
    public class WaveNode
    {
        public int xcoord;
        public int ycoord;
        public int index;

        public float thresh;
    }
}