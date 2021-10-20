using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunkController : BaseProjectile
{    
    public static bool canMove;
    [SerializeField] private float force = 10f;

    protected override void Update()
    {
        base.Update();

        if (canMove)
        {
           //SoundManager.PlaySound(SoundManager.Sound.Turret, 1f); //- Error ...why?????1019_BR
            rigidbody.velocity = transform.up * force;
        }
    }

    public override void OnHitEntity() {
        this.Despawn();
    }
}
