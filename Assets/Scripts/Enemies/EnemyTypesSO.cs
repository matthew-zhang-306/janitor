using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu()]
public class EnemyTypesSO : ScriptableObject
{
    [System.Serializable]
    public class EnemyType {
        public string name;
        public Color color;
        public GameObject prefab;

        public EnemyType(string n, Color c, GameObject p) {
            name = n;
            color = c;
            prefab = p;
        }
    }

    [SerializeField] private EnemyType[] enemyTypes;

    public Dictionary<string, EnemyType> GetEnemyTypes() {
        var dict = new Dictionary<string, EnemyType>();
        foreach (EnemyType e in enemyTypes) {
            dict[e.name] = e;
        }
        return dict;
    }
}
