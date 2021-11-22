using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class RoomManagerManager : MonoBehaviour
{
    //Manages Room Managers
    public RoomManager[] rooms;
    public Checkpoint[] checkpoints;

    //make save state for every room?

    void Start ()
    {
        //this.gameObject.GetInstanceID()
    }

    void OnCheckPoint ()
    {
        foreach (var room in rooms) {

            //Save Floor Schema 
            room.dirtyTiles.SaveFloor();
        }
    }

    private class RoomSaveState
    {
        FloorController.FloorData floorSave;
        
    }
}