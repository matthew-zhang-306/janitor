using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyCollectable : Interactable
{
    public int amount = 1;
    void Start ()
    {
        autoInteract = true;
    }
    public virtual void DoAction (PlayerController pc, Inventory i)
    {
        i.money += amount;
        Destroy (gameObject);
    }
}