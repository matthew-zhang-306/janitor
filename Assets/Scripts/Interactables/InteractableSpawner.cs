using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class InteractableSpawner : MonoBehaviour
{
    //NOTE this script should be attached to the game object that is *parent* to all rooms and other things that decide 
    //that they want to spawn something in
    //Reason it is not placed inside a room is that a hallway might have a cabinet where if you interact
    //It would spawn health / ammo so

    public GameObject InteractablePrefabs;

    void Start ()
    {

    }

    public void SpawnItem (int index, Vector3 position)
    {

    }
}