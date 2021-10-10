using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

[RequireComponent (typeof(BaseEnemy))]
public class EnemyLootTable : BaseLootTable
{
    private BaseEnemy be;
    protected override void Start ()
    {
        base.Start();
        be = this.GetComponent<BaseEnemy>();

        be.onDeath += SpawnLootFromTable;
    }

    void SpawnLootFromTable (BaseEnemy be)
    {
        float value = Random.Range (0f, 1f);
        float runningTotal = 0f;
        foreach (var lv in lootTable) {
            
            if (lv.probability == 0) continue;
            runningTotal += lv.probability;
            if (value <= runningTotal) {
                
                InteractableSpawner.i.SpawnItem (lv.name, be.transform.position);
                return;
            }
        }
    }
}
