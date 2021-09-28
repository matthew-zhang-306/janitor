using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectable : Interactable
{
    public int healthAmount = 10;
    // Start is called before the first frame update

    public override void DoAction (PlayerController pc)
    {
        var health = pc.GetComponent<Health>();

        health?.ChangeHealth (healthAmount);
        Destroy (gameObject);
    }
    
}
