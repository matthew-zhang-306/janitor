using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectable : Interactable
{
    public int healthAmount = 10;
    // Start is called before the first frame update
    public override string ToolTip
    {
        get => _tooltip + "(" + healthAmount + ")";
    }
    public override void DoAction (PlayerController pc, Inventory _)
    {
        var health = pc.GetComponent<Health>();

        health?.ChangeHealth (healthAmount);
        Destroy (gameObject);
    }
    
}
