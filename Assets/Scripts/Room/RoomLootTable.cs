using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

[RequireComponent (typeof(RoomManager))]
public class RoomLootTable : LootTable
{
    private RoomManager rm;
    protected override void Start ()
    {
        base.Start();
        rm = this.GetComponent<RoomManager>();

        rm.onRoomClear += SpawnLootFromTable;
    }

    void SpawnLootFromTable (PlayerController pc, RoomManager rm)
    {
        float value = Random.Range (0f, 1f);

        float runningTotal = 0f;
        foreach (var lv in lootTable) {
            
            if (lv.probability == 0) continue;
            runningTotal += lv.probability;
            if (value <= runningTotal) {
                InteractableSpawner.i.SpawnItem (lv.name, pc.transform.position);
                break;
            }
        }
    }
}
