using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class LootTable : MonoBehaviour
{
    [System.Serializable]
    protected class LootValue
    {
        public string name;
        public float probability;
    }

    [SerializeField] protected LootValue[] lootTable;

    protected virtual void Start()
    {
        //Normalize loot table
        if (lootTable.Length != 0) {
            float totalProb = 0;
            foreach (var lv in lootTable) {
                totalProb += lv.probability;
            }

            if (totalProb == 0) {
                //In case all probs are 0, ignore and think that they are 1 instead
                totalProb = 1f / lootTable.Length;
                foreach (var lv in lootTable) {
                    lv.probability = totalProb;
                }
            }
            else {
                foreach (var lv in lootTable) {
                    lv.probability /= totalProb;
                }
            }
            
        }
    }
}