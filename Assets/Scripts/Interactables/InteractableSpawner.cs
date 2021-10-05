using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class InteractableSpawner : MonoBehaviour
{
    //Is a SINGLETON instance.

    private static InteractableSpawner _i;

    public static InteractableSpawner i
    {
        get
        {
            if (_i == null) _i = (Instantiate(Resources.Load("InteractSpawn")) as GameObject).GetComponent<InteractableSpawner>();
            return _i;
        }
    }

    public ItemTypesSO itemTypes;
    public GameObject[] interactablePrefabs;
    private Dictionary<string, GameObject> prefabDict;

    private GameObject iPool;
    void Awake ()
    {
        iPool = new GameObject ("Interactables");
        iPool.transform.SetParent(transform);
        prefabDict = itemTypes?.GetItemDict() ?? new Dictionary<string, GameObject>();

    }

    public void SpawnItem (string index, Vector3 position)
    {
        Debug.Log (position);
        var go = Instantiate (prefabDict[index], position, Quaternion.identity);
        // go.transform.SetParent (iPool.transform);
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