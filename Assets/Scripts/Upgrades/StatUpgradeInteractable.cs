using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StatUpgradeInteractable : Interactable
{
    [SerializeField] public Upgradeable context;

    [SerializeField] private Upgrade[] ulist;
    [SerializeField] private Sprite image;


    void Start ()
    {
        autoInteract = false;
    }

    public override void DoAction (PlayerController pc, Inventory i)
    {
        this.GetComponent<Collider2D>().enabled = false;


        foreach (Upgrade u in ulist)
        {
            i.ApplyUpgrade(u, context.GetType());
        }
        
        i.SaveUpgrade(ulist, context.GetType());

        //Don't destroy as it fucks with a few references
        gameObject.SetActive(false);

    }
}