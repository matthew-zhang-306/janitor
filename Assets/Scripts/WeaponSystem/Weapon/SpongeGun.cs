using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SpongeGun : BaseWeapon
{
    public Animator muzzleAnimation;

    public override float HandleFire(Vector3 dir, Quaternion rotation)
    {
        GameObject go;
        base.HandleFire(dir, rotation, out go);
        
        go.GetComponent<BaseProjectile>().hurtbox.gameObject.GetComponent<Damage>().damage = (int) this.bulletDamage;
        muzzleAnimation.SetTrigger("active");
        return ammoDrain;
    }
}