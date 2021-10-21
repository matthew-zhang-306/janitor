using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnMarker : MonoBehaviour
{
    GameObject enemyPrefab;
    RoomManager roomManager;

    public void SetEnemy(GameObject prefab, RoomManager room) {
        enemyPrefab = prefab;
        roomManager = room;
        GetComponent<Animator>().Play("Spawn");
    }

    public void SpawnEnemy() {
        GameObject created = Instantiate(enemyPrefab, transform.position, Quaternion.identity, transform.parent);
        roomManager.InitEnemy(created.transform, true);
    }

    public void OnFinish() {
        Destroy(gameObject);
    }
}
