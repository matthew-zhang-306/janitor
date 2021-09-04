using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float maxSpeed = 1f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2d;
    private Hitbox hitbox;
    private HealthAddon health;

    private Vector2 knockback;
    private float knockbackTimer;
    private float invincibilityTimer;
    [Header("Getting Hit")]
    [SerializeField] private float knockbackPower = 1f;
    [SerializeField] private float knockbackTime = 1f;
    [SerializeField] private float knockbackFriction = 1f;
    [SerializeField] private float invincibilityTime = 1f;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        hitbox = GetComponentInChildren<Hitbox>();
        health = GetComponent<HealthAddon>();
    }

    private void Update() {
        invincibilityTimer = Mathf.MoveTowards(invincibilityTimer, 0, Time.deltaTime);
        spriteRenderer.color = invincibilityTimer > 0 ? new Color(1, 0.4f, 0.4f) : Color.white;

        if (hitbox.IsColliding && invincibilityTimer == 0) {
            health.ChangeHealth(-1);
            if (health.GetHealth() <= 0) {
                Destroy(gameObject);
            }

            Vector2 knockbackDir = transform.position - hitbox.OtherCollider.transform.position;
            knockbackDir = knockbackDir.normalized;
            knockback = knockbackPower * knockbackDir;
            knockbackTimer = knockbackTime;

            invincibilityTimer = invincibilityTime;
        }
    }

    private void FixedUpdate() {
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
