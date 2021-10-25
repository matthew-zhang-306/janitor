using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatUpgradeInteractable : Interactable
{
    private Upgradeable context;

    void Start ()
    {
        autoInteract = false;
    }

    public override void DoAction (PlayerController pc, Inventory i)
    {
        this.GetComponent<Collider2D>().enabled = false;

        pc.ApplyUpgrade(new Upgrade("maxSpeed", 10));
        
        Destroy (gameObject);
    }
}