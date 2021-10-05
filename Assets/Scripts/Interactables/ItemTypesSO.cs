using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemTypesScriptableObject", order = 1)]
public class ItemTypesSO : ScriptableObject
{
    [System.Serializable]
    public class Item {
        public string name;
        public GameObject prefab;

        public Item(string n, GameObject p) {
            name = n;
            prefab = p;
        }
    }

    [SerializeField] private Item[] itemTypes;

    public Dictionary<string, GameObject> GetItemDict() {
        var dict = new Dictionary<string, GameObject>();
        foreach (Item e in itemTypes) {
            dict.Add (e.name, e.prefab);
        }
        return dict;
    }
}
