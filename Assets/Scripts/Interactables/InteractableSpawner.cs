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

    public GameObject GetItem (string index)
    {
        if (index == "") return null;

        try {
            return prefabDict[index];
        }
        catch {
            Debug.LogError ("Invalid Item Index! (Are you using the editor / item SO?)");
            return null;
        }
        
    }

    public void SpawnItem (string index, Vector3 position)
    {
        if (index.Equals("") || index == null) return;
        Debug.Log ("Spawning: " + index);
        var go = Instantiate (prefabDict[index], position, Quaternion.identity);
        go.transform.SetParent (iPool.transform);
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