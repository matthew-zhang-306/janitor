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
    public override void DoAction (PlayerController pc, Inventory i)
    {
        if (!this.GetComponent<Collider2D>().enabled) return;
        this.GetComponent<Collider2D>().enabled = false;

        i.money += amount;
        SoundManager.PlaySound(SoundManager.Sound.Coin, 1f);

        Destroy (gameObject);
    }
}