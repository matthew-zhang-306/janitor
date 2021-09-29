using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class SpongeProjectile : BaseProjectile
{
    public Animator anim;
    public Hitbox hurtbox;
    public SpriteRenderer sr;
    private Rigidbody2D rb2d;

    protected override void OnEnable()
    {
        base.OnEnable();
        anim.gameObject.SetActive(false);

        sr.enabled = true;
        wallHitbox.enabled = true;
        hurtbox.enabled = true;
        hurtbox.gameObject.SetActive(true);
    }

    protected override void Start()
    {
        base.Start();
        rb2d = this.GetComponent<Rigidbody2D>();
    }

    public override void OnHitEntity() {
        StartCoroutine(PlayAnim());
    }

    protected override void OnHitWall(Collider2D _)
    {
        StartCoroutine(PlayAnim());
    }

    private IEnumerator PlayAnim()
    {
        anim.gameObject.SetActive(true);
        rb2d.velocity = Vector2.zero;
        sr.enabled = false;

        hurtbox.gameObject.SetActive(false);
        wallHitbox.enabled = false;

        yield return new WaitForSeconds(1);
        
        this.OnDespawn();
    }
}