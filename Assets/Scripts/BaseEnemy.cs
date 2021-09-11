using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class BaseEnemy : MonoBehaviour
{
    [HideInInspector] public bool CanAct = false;

    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb2d;
    protected Health health;
    public Hitbox hitbox;
    

    [Header("Optional Components")]
    public GameObject floorMarker; // enemy will not necessarily have one
    protected NavMeshAgent navMeshAgent; // enemy will not necessarily have one

    [HideInInspector] public PlayerController player;

    protected UnityEvent deathEvent;
    public UnityEvent DeathEvent => deathEvent; // note: change to c# delegate? unityevent is slow

    [Header("Parameters")]
    [SerializeField] protected float invincibilityTime = 0.2f;
    protected float invincibilityTimer;


    protected virtual void Awake() {
        if (deathEvent == null)
            deathEvent = new UnityEvent();
    }

    protected virtual void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null) {
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
        }
        navMeshAgent.isStopped = true;

        health = GetComponent<Health>();
        rb2d = GetComponent<Rigidbody2D>();
    }


    protected virtual void FixedUpdate() {
        invincibilityTimer = Mathf.MoveTowards(invincibilityTimer, 0f, Time.deltaTime);
        if (health != null && hitbox.IsColliding && invincibilityTimer == 0f) {
            // take a hit
            TakeDamage(hitbox.OtherCollider);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D col) {
        if (health != null &&
            col.gameObject.CompareTag(hitbox.targetTags[0]) &&
            invincibilityTimer == 0f)
        {
            TakeDamage(col.collider);
        }
    }


    protected virtual void TakeDamage(Collider2D other) {
        int hitAmount = 1;
        var damage = other.GetComponent<Damage>();
        if (damage != null) {
            hitAmount = damage.damage;
        }
        health.ChangeHealth(-hitAmount);

        invincibilityTimer = invincibilityTime;

        if (health.GetHealth() <= 0) {
            CanAct = false;
            Die();

            // i call the deathevent here instead of in Die() because i expect subclasses to not call base.Die()
            // (since the default implementation destroys the object with no animation)
            deathEvent.Invoke();
            deathEvent.RemoveAllListeners();
        }
    }

    protected virtual void Die() {
        Destroy(gameObject);
    }
}
