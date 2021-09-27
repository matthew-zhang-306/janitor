using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class RoomComponentCopy : MonoBehaviour
{
    //Put this into gameobjects which are direct children to the Room prefab
    //if you wish to have it reset during a checkpoint
    private GameObject copy;

    void Start ()
    {
        
    }

    public void CreateCopy ()
    {
        copy = Instantiate(gameObject, transform.parent);
        copy.SetActive (false);
    }

    public RoomComponentCopy Replace ()
    {
        copy.SetActive (true);
        Destroy(gameObject, (float) 1e-5);    
        return copy.GetComponent<RoomComponentCopy>();
    }
}