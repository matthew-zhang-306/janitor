using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public delegate void PlayerDeath ();


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float maxSpeed = 1f;
    [SerializeField] private WeaponSystem weapon;
    public Transform cameraPos;
    public SpriteRenderer spriteRenderer;
    public AudioSource damageSound;
    private Animator animator;
    private Rigidbody2D rb2d;
    private Hitbox hitbox;
    private Health health;

    //Event for death
    public PlayerDeath onDeath;
    private PlayerSnapShot previousPss;


    private Vector2 knockback;
    private float knockbackTimer;
    private float invincibilityTimer;

    private float dashTimer;
    [Header("Dash")]
    [Tooltip("How long a dash is (ignores input while dashing)")]
    [SerializeField] private float dashTime = 0.1f;
    [Tooltip("Time between dashes")]
    [SerializeField] private float dashCooldown = 3f;
    [Tooltip("How many seconds should the player be invurnurable")]
    [SerializeField] private float dashIframe = 0.3f;
    [Tooltip("How strong is the dash")]
    [SerializeField] private float dashSpeedBonus = 50f;

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
        dashCooldown += dashTime;

        hitbox.OnTriggerEnter.AddListener(OnEnterHazard);

        previousPss = SnapShot();
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

        // iframae indicator (needs to be replaced with something better looking)
        spriteRenderer.color = invincibilityTimer > 0 ? new Color(1, 0.4f, 0.4f) : Color.white;
    }

    private void FixedUpdate() {
        HandleMotion();
        invincibilityTimer = Mathf.MoveTowards(invincibilityTimer, 0, Time.deltaTime);
    }

    private void OnEnterHazard(Collider2D other) {
        other.GetComponentInParent<BaseProjectile>()?.OnHitEntity();
            
        if (invincibilityTimer == 0f) {
            // take a hit
            TakeDamage(hitbox.OtherCollider);
        }
    }

    private void TakeDamage(Collider2D other) {
        var damage = other.GetComponent<Damage>();
        if (damage == null) {
            Debug.LogWarning("Why in hitbox if damage be 0 or null (in player controller)");
        }

        //Should be set to 0, but for debug purposes
        int hitAmount = damage?.damage ?? 1;            

        health.ChangeHealth(-hitAmount);
        damageSound.Play();
        if (health.GetHealth() <= 0) {
            //call event
            onDeath?.Invoke();
        }

        // handle knockback
        Vector2 knockbackDir = transform.position - other.transform.position;
        knockbackDir = knockbackDir.normalized;
        knockback = knockbackPower * knockbackDir;
        knockbackTimer = knockbackTime;

        invincibilityTimer = invincibilityTime;
    }

    private void HandleMotion() {
        dashTimer -= Time.deltaTime;

        if (dashTimer > dashCooldown - dashTime) {
            return;
        }

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

        //Dash stuff
        
        
        // apply this velocity to the player
        Vector2 velocity = rb2d.velocity;
        velocity = Vector2.MoveTowards(velocity, moveInput * (maxSpeed), acceleration);
        rb2d.velocity = velocity;
        
        bool isDash = Input.GetButton("Jump");
        if (isDash && dashTimer <= 0f) {       
            var col = Physics2D.OverlapPoint(transform.position + (moveInput * dashSpeedBonus * dashTime).ToVector3(), LayerMask.GetMask("Hole"));     
            if (col != null) {
                return;
            }
            DisableEnemyCollision();

            rb2d.AddForce(moveInput * dashSpeedBonus, ForceMode2D.Impulse);
            dashTimer = dashCooldown;
            Invoke("EnableEnemyCollision", dashTime);
            //might fuck things up
            invincibilityTimer += dashIframe;
        }
    }

    private void DisableEnemyCollision ()
    {
        Physics2D.IgnoreLayerCollision(8, 14, true);
        Physics2D.IgnoreLayerCollision(8, 10, true);
    }
    private void EnableEnemyCollision ()
    {   
        Physics2D.IgnoreLayerCollision(8, 14, false);
        Physics2D.IgnoreLayerCollision(8, 10, false);
    }
    public float DashCooldownUI()
    {
        return dashTimer;
    }

    public PlayerSnapShot SnapShot()
    {
        var pss = new PlayerSnapShot (transform, health, weapon);
        previousPss = pss;
        return pss;
    }

    public void ResetFromPrevious ()
    {
        previousPss?.Apply(this);
    }

    public class PlayerSnapShot
    {
        public readonly Vector3 position;
        public readonly int health;
        public readonly int maxHealth;

        public PlayerSnapShot (Transform playerTransform, Health playerHealth, WeaponSystem playerWeapon)
        {
            position = playerTransform.position;
            health = playerHealth.GetHealth();
            maxHealth = playerHealth.GetMaxHealth();

            //Add weapon ammo and stuff here!


        }

        public void Apply (PlayerController pc) 
        {
            pc.transform.position = this.position;
            //don't ask
            pc.health.ChangeHealth (-pc.health.GetHealth() + health);

            pc.health.SetMaxHealth (maxHealth);
        }
    }
}
