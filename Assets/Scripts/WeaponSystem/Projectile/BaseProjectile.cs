using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class BaseProjectile : MonoBehaviour
{
    public Hitbox wallHitbox;
    public Collider2D hurtbox;

    public float lifetime = 1f;
    private float m_time = 0f;

    protected new Rigidbody2D rigidbody;
    

    protected virtual void OnEnable()
    {
        m_time = 0f;
        wallHitbox.OnTriggerEnter.AddListener(OnHitWall);
    }
    protected virtual void OnDisable()
    {
        wallHitbox.OnTriggerEnter.RemoveListener(OnHitWall);
    }

    protected virtual void Start ()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    
    protected virtual void Update()
    {
        m_time += Time.deltaTime;
        if (m_time > lifetime) {
            this.Despawn();
        }
    }

    void OnDestroy()
    {
        // turning off this warning because all bullets are destroyed when the scene unloads
        // Debug.LogWarning("bullet should not be destroyed usually");
        this.Despawn();
    }

    
    public virtual void OnHitEntity() {
        // by default, do nothing
    }

    protected virtual void Despawn() {
        // a projectile pooler will collect this object
        gameObject.SetActive(false);
    }

    protected virtual void OnHitWall(Collider2D _) {
        this.Despawn();
    }
}
