using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

[RequireComponent (typeof(RoomManager))]
public class RoomLootTable : MonoBehaviour
{
    [System.Serializable]
    public class LootValue
    {
        public string name;
        public float probability;
    }

    [SerializeField] private LootValue[] lootTable;

    private RoomManager rm;
    void Start ()
    {
        rm = this.GetComponent<RoomManager>();

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

        rm.onRoomClear += SpawnLootFromTable;
    }

    void SpawnLootFromTable (PlayerController pc, RoomManager rm)
    {
        
        float value = Random.Range (0f, 1f);

        float runningTotal = 0f;
        foreach (var lv in lootTable) {
            runningTotal += lv.probability;
            if (value <= runningTotal) {
                Debug.Log ("beep beep");
                InteractableSpawner.i.SpawnItem (lv.name, pc.transform.position);
                break;
            }
        }
    }
}
