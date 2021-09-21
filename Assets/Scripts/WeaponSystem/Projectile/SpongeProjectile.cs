using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof (FloorMarker))]
public class SpongeProjectile : BaseProjectile
{
    public Animator anim;
    private SpriteRenderer sr;
    private Rigidbody2D rb2d;
    private Collider2D selfCol;
    protected override void OnEnable()
    {
        base.OnEnable();
        anim.gameObject.SetActive(false);

        if (sr != null) {
            sr.enabled = true;
        }
        if (selfCol != null) {
            selfCol.enabled = true;
        }

    }
    protected override void Start()
    {
        base.Start();
        sr = this.GetComponent<SpriteRenderer>();
        rb2d = this.GetComponent<Rigidbody2D>();
        selfCol = this.GetComponent<BoxCollider2D>();
    }
    protected virtual void OnTriggerEnter2D(Collider2D col) {
        
        if (col.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(PlayAnim());
        }
    }

    private IEnumerator PlayAnim()
    {
        anim.gameObject.SetActive(true);
        rb2d.velocity = Vector2.zero;
        
        sr.enabled = false;
        selfCol.enabled = false;

        yield return new WaitForSeconds(1);
        
        this.OnDespawn();
    }
}