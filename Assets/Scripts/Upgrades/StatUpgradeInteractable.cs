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

        // Destroy(this.GetComponent<Collider2D>());

        foreach (Upgrade u in ulist)
        {
            i.ApplyUpgrade(u, context);
        }
        
        i.SaveUpgrade(this);

        gameObject.SetActive(false);

        // Destroy (gameObject);
    }
}