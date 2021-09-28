using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent (typeof (PlayerController))]
public class Inventory : MonoBehaviour
{
    private PlayerController pc;

    private int _money;
    public int money {get => _money; set => Mathf.Clamp(_money + value, 0, 1000);}


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
            recent.FirstOrDefault()?.DoAction(pc, this);
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        var item = other.GetComponent<Interactable>();
        if (item != null) {
            if (item.autoInteract)
                item.DoAction (pc, this);
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