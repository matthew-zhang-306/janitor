using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class InteractableSpawner : MonoBehaviour
{
    //NOTE this script should be attached to the game object that is *parent* to all rooms and other things that decide 
    //that they want to spawn something in
    //Reason it is not placed inside a room is that a hallway might have a cabinet where if you interact
    //It would spawn health / ammo so

    public GameObject InteractablePrefabs;
    private Dictionary<string, GameObject> prefabDict;

    private GameObject iPool;
    void Start ()
    {
        iPool = new GameObject ("Interactables");
        iPool.transform.SetParent(transform);
    }

    public void SpawnItem (string index, Vector3 position)
    {
        Instantiate (prefabDict[index], position, Quaternion.identity, iPool.transform);
    }

    public void SpawnRandomItem (Vector3 position)
    {
        SpawnItem (prefabDict.Keys.ToList()[Random.Range(0, prefabDict.Count)], position);
    }

    public void SpawnRandomItemOnPlayer ()
    {
        GameObject player = GameObject.Find("Player");
        SpawnRandomItem (player.transform.position);
    }
}