using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent (typeof (PlayerController))]
public class Inventory : MonoBehaviour
{
    private PlayerController pc;

    private List<Interactable> recent;
    void Start () 
    {
        pc = this.GetComponent<PlayerController>();
        recent = new List<Interactable>();
    }

    void Update ()
    {
        if (Input.GetButton("Interact"))
        {
            recent.FirstOrDefault()?.DoAction(pc);
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        var item = other.GetComponent<Interactable>();
        if (item != null) {
            if (item.autoInteract)
                item.DoAction (pc);
            else {
                recent.Add (item);
            }
        }
    }

    void OnTriggerExit2D (Collider2D other)
    {
        var item = other.GetComponent<Interactable>();
        if (item != null) {
            recent.Remove (item);
        }
    }
}