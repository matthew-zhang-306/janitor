using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SpongeGun : BaseWeapon
{
    private Animator gunAnimation;
    protected override void Start()
    {
        base.Start();
        gunAnimation = transform.GetChild(0).GetComponent<Animator>();
    }
    public override float HandleFire(Vector3 dir, Quaternion rotation)
    {
        base.HandleFire(dir, rotation);
        gunAnimation.SetTrigger("active");
        return ammoDrain;
    }
}