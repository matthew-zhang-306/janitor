using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class ShopItemLootTable : BaseLootTable
{
    protected override void Start ()
    {
        base.Start();
        
    }

    // void SpawnLootFromTable ()
    // {
    //     float value = Random.Range (0f, 1f);
    //     float runningTotal = 0f;
    //     foreach (var lv in lootTable) {
            
    //         if (lv.probability == 0) continue;
    //         runningTotal += lv.probability;
    //         if (value <= runningTotal) {
                
    //             InteractableSpawner.i.SpawnItem (lv.name, be.transform.position);
    //             return;
    //         }
    //     }
    // }

    public string GetItem ()
    {
        float value = Random.Range (0f, 1f);
        float runningTotal = 0f;
        foreach (var lv in lootTable) {
            
            if (lv.probability == 0) continue;
            runningTotal += lv.probability;
            if (value <= runningTotal) {
                
                // InteractableSpawner.i.SpawnItem (lv.name, be.transform.position);
                return lv.name;
            }
        }
        return null;
    }
}
