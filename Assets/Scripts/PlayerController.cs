using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float maxSpeed = 1f;

    public Transform cameraPos;
    public SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb2d;
    private Hitbox hitbox;
    private Health health;
    public static bool PlayerDead;

    private Vector2 knockback;
    private float knockbackTimer;
    private float invincibilityTimer;
    [Header("Getting Hit")]
    [SerializeField] private float knockbackPower = 1f;
    [SerializeField] private float knockbackTime = 1f;
    [SerializeField] private float knockbackFriction = 1f;
    [SerializeField] private float invincibilityTime = 1f;

    private void Start() {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        hitbox = GetComponentInChildren<Hitbox>();
        health = GetComponent<Health>();
        PlayerDead = false;
    }

    private void Update() {
        var animation = "Idle";
        if (rb2d.velocity.magnitude >= acceleration) {
            animation = "Run";
            if (Mathf.Abs(rb2d.velocity.x) >= Mathf.Abs(rb2d.velocity.y)) {
                animation += Mathf.Sign(rb2d.velocity.x) > 0 ? "Right" : "Left";
            } else {
                animation += Mathf.Sign(rb2d.velocity.y) > 0 ? "Up" : "Down";
            }
        }

        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != animation) {
            animator.Play("Player" + animation, 0);
        }
    }

    private void FixedUpdate() {
        HandleHitbox();
        HandleMotion();
    }


    private void HandleHitbox() {
        invincibilityTimer = Mathf.MoveTowards(invincibilityTimer, 0, Time.deltaTime);
        spriteRenderer.color = invincibilityTimer > 0 ? new Color(1, 0.4f, 0.4f) : Color.white;

        if (hitbox.IsColliding && invincibilityTimer == 0) {
            int hitAmount = 1;
            var damage = hitbox.OtherCollider.GetComponent<Damage>();
            if (damage != null) {
                hitAmount = damage.damage;
            }
            
            health.ChangeHealth(-hitAmount);
            if (health.GetHealth() <= 0) {
                PlayerDead = true;
                Destroy(gameObject);
            }

            Vector2 knockbackDir = transform.position - hitbox.OtherCollider.transform.position;
            knockbackDir = knockbackDir.normalized;
            knockback = knockbackPower * knockbackDir;
            knockbackTimer = knockbackTime;

            invincibilityTimer = invincibilityTime;
        }
    }

    private void HandleMotion() {
        if (knockbackTimer > 0f) {
            // handle knockback
            knockbackTimer -= Time.deltaTime;
            rb2d.velocity = knockback;
            knockback = Vector2.MoveTowards(knockback, Vector2.zero, knockbackFriction);
            return;
        }

        // find the direction that the player is moving
        Vector2 moveInput = Vector2.zero;
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // apply this velocity to the player
        Vector2 velocity = rb2d.velocity;
        velocity = Vector2.MoveTowards(velocity, moveInput * maxSpeed, acceleration);
        rb2d.velocity = velocity;
    }

    
}
