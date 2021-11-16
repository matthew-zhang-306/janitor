using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using System;
using System.Reflection;
using UnityEditor;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : Upgradeable
{
    public delegate void PlayerEvent(PlayerController player);

    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float maxSpeed = 1f;

    public WeaponSystem weapon;
    public Health health;
    public Inventory inventory;


    public Transform cameraPos;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer shadowRenderer;
    public SpriteFlash spriteFlash;
    public AudioSource damageSound;
    private Animator animator;
    private Rigidbody2D rb2d;
    private new Collider2D collider;
    private Hitbox[] hitboxes;


    public Vector2 Velocity => rb2d?.velocity ?? Vector2.zero;
    

    private bool isDead;
    public static PlayerEvent OnDash;
    public static PlayerEvent OnHitCheckpoint;
    public static PlayerEvent OnDeath;
    public static PlayerEvent OnRestart;
    private PlayerSnapShot checkpointSnapshot;

    private Vector2 previousMoveInput;
    private Vector2 knockback;
    private float knockbackTimer;
    private float invincibilityTimer;

    private float dashTimer;
    private bool isDashing => dashTimer > dashCooldown - dashTime;
    public float DashTime => dashTime;

    [Header("Dash")]
    [Tooltip("How long a dash is (ignores input while dashing)")]
    [SerializeField] private float dashTime = 0.1f;
    [Tooltip("Time between dashes")]
    [SerializeField] private float dashCooldown = 3f;
    [Tooltip("How many seconds should the player be invurnurable")]
    [SerializeField] private float dashIframe = 0.3f;
    [Tooltip("How strong is the dash")]
    [SerializeField] private float dashSpeed = 50f;

    [Header("Getting Hit")]
    [SerializeField] private float knockbackPower = 1f;
    [SerializeField] private float knockbackTime = 1f;
    [SerializeField] private float knockbackFriction = 1f;
    [SerializeField] private float invincibilityTime = 1f;

    private float shadowBaseAlpha;


    private void Start() {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        hitboxes = GetComponentsInChildren<Hitbox>();
        health = GetComponent<Health>();
        dashCooldown += dashTime;

        foreach (Hitbox hitbox in hitboxes)
            hitbox.OnTriggerEnter.AddListener(OnEnterHazard);

        checkpointSnapshot = new PlayerSnapShot(transform, health, weapon, inventory);
        
        shadowBaseAlpha = shadowRenderer.color.a;
    }

    private void Update() {
        if (isDead) {
            return;
        }

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
        dashTimer = Mathf.MoveTowards(dashTimer, 0f, Time.deltaTime);
        invincibilityTimer = Mathf.MoveTowards(invincibilityTimer, 0, Time.deltaTime);

        if (isDead) {
            rb2d.velocity = Vector2.zero;
            return;
        }

        if (dashTimer <= dashCooldown - dashTime) {
            HandleMotion();
        }
    }

    private void OnEnterHazard(Collider2D other) {
        other.GetComponentInParent<BaseProjectile>()?.OnHitEntity();
            
        if (invincibilityTimer == 0f && !isDashing) {
            // take a hit
            TakeDamage(other);
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
        SoundManager.PlaySound(SoundManager.Sound.Damage, 0.6f);
        if (health.GetHealth() <= 0) {
            StartCoroutine(Die());
        }

        // handle knockback
        Vector2 knockbackDir = transform.position - other.transform.position;
        knockbackDir = knockbackDir.normalized;
        knockback = knockbackPower * knockbackDir;
        knockbackTimer = knockbackTime;

        invincibilityTimer = invincibilityTime;
        if (health.GetHealth() > 0) {
            spriteFlash.Flash(invincibilityTime - 0.1f, -knockbackDir.x, knockbackTime);
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
        moveInput.x = CustomInput.GetAxis("Horizontal");
        moveInput.y = CustomInput.GetAxis("Vertical");
        
        moveInput = moveInput.normalized;
        if (moveInput.sqrMagnitude > 0.1f) {
            previousMoveInput = moveInput;
        }
        
        // apply this velocity to the player
        Vector2 velocity = rb2d.velocity;
        velocity = Vector2.MoveTowards(velocity, moveInput * (maxSpeed), acceleration);
        rb2d.velocity = velocity;
        
        // handle dashing
        bool isDash = CustomInput.GetButton("Jump");
        if (isDash && dashTimer <= 0f) {
            StartCoroutine(DoDash(moveInput));
        }
    }

    private IEnumerator DoDash(Vector2 dashDirection) {
        if (dashDirection == Vector2.zero) {
            dashDirection = previousMoveInput.normalized;
        }

        Vector3 dashPoint = CalculateDashPoint(dashDirection, out Vector3 lastHolePosition);

        bool collisionDisabled = false;
        if (!collider.bounds.Contains(lastHolePosition)) {
            // this dash may cross over holes
            collisionDisabled = true;
            collider.isTrigger = true;
        }

        OnDash?.Invoke(this);

        dashTimer = dashCooldown;
        while (dashTimer > dashCooldown - dashTime) {
            if (collisionDisabled && collider.bounds.Contains(lastHolePosition)) {
                // done with the holes, let the rest of the dash play out with collision on
                collider.isTrigger = false;
            }
            
            rb2d.velocity = dashDirection * dashSpeed;
            yield return new WaitForFixedUpdate();
        }

        collider.isTrigger = false;
        rb2d.velocity = dashDirection * maxSpeed;
    }

    private Vector3 CalculateDashPoint(Vector2 dashDirection, out Vector3 lastHolePosition) {
        Vector2 maxDashPoint = collider.bounds.center + dashDirection.ToVector3() * dashSpeed * dashTime;
        Vector2 desiredDashPoint = maxDashPoint;

        // do not consider anything past the first wall
        RaycastHit2D hit = Physics2D.Raycast(
            collider.bounds.center,
            dashDirection,
            dashSpeed * dashTime,
            LayerMask.GetMask("Wall", "Sides")
        );
        if (hit.collider != null) {
            desiredDashPoint = hit.point - dashDirection * 0.1f;
        }

        // todo: get this grid size from somewhere
        float gridSize = 0.5f;
        int i = 0;

        // step backward one tile at a time
        Vector2 backDirection = -dashDirection;
        Vector2 gridPoint = Vector3.zero;
        while (Vector3.Distance(desiredDashPoint, collider.bounds.center) > 0.1f &&
               Physics2D.OverlapPoint(desiredDashPoint, LayerMask.GetMask("Hole")) != null) {
            Debug.DrawLine(transform.position + Vector3.up * (i++), desiredDashPoint, Color.magenta, 5f);

            gridPoint = GetNextGridIntersection(
                desiredDashPoint, backDirection,
                Vector3.Distance(desiredDashPoint, collider.bounds.center),
                gridSize
            );
            
            Debug.DrawLine(transform.position + Vector3.up * (i - 0.5f), desiredDashPoint, Color.red, 5f);
            desiredDashPoint = gridPoint + backDirection * 0.09f;
            Debug.DrawLine(transform.position + Vector3.up * i, desiredDashPoint, Color.magenta, 5f);
        }

        // set lastHolePosition to the midpoint between the desired dash point and the next horizontal/vertical down the line
        gridPoint = GetNextGridIntersection(
            desiredDashPoint, backDirection,
            Vector3.Distance(desiredDashPoint, collider.bounds.center),
            gridSize
        );
        lastHolePosition = new Vector3((desiredDashPoint.x + gridPoint.x) / 2f, (desiredDashPoint.y + gridPoint.y) / 2f, 0);

        return desiredDashPoint.ToVector3();
    }

    private Vector2 GetNextGridIntersection(Vector2 fromPos, Vector2 rayDir, float maxDist, float gridSize) {
        float dNextHorizontal = maxDist;
        float dNextVertical = maxDist;
        float tNextHorizontal = maxDist;
        float tNextVertical = maxDist;

        // find the next vertical line
        if (rayDir.x > 0)
            dNextVertical = Mathf.Min(dNextVertical, Mathf.Floor(fromPos.x / gridSize + 1f) * gridSize - fromPos.x);
        else if (rayDir.x < 0)
            dNextVertical = Mathf.Min(dNextVertical, fromPos.x - Mathf.Ceil(fromPos.x / gridSize - 1f) * gridSize);

        // find the next horizontal line
        if (rayDir.y > 0) 
            dNextHorizontal = Mathf.Min(dNextHorizontal, Mathf.Floor(fromPos.y / gridSize + 1f) * gridSize - fromPos.y);
        else if (rayDir.y < 0)
            dNextHorizontal = Mathf.Min(dNextHorizontal, fromPos.y - Mathf.Ceil(fromPos.y / gridSize - 1f) * gridSize);

        // find times to each each line
        if (rayDir.x != 0)
            tNextVertical = dNextVertical / Mathf.Abs(rayDir.x);
        if (rayDir.y != 0)
            tNextHorizontal = dNextHorizontal / Mathf.Abs(rayDir.y);
        
        return fromPos + rayDir * Mathf.Min(tNextHorizontal, tNextVertical);
    }

    public float GetDashTimer()
    {
        return dashTimer;
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Checkpoint")) {
            checkpointSnapshot = new PlayerSnapShot(transform, health, weapon, inventory);
            OnHitCheckpoint?.Invoke(this);
        }
    }


    public IEnumerator Die() {
        isDead = true;
        
        animator.Play("PlayerDeath");
        SoundManager.PlaySound(SoundManager.Sound.PlayerDeath, 1.3f);
        foreach (Transform child in transform) {
            if (child.name == "Visuals") {
                // keep the visuals of the player on
                continue;
            }
            child.gameObject.SetActive(false);
        }
        
        OnDeath?.Invoke(this);

        yield return new WaitForSeconds(2f);

        // reset the player
        checkpointSnapshot?.Apply(this);
        foreach (Transform child in transform) {
            child.gameObject.SetActive(true);
        }
        isDead = false;
        spriteRenderer.DOKill();
        spriteRenderer.color = spriteRenderer.color.WithAlpha(1);
        shadowRenderer.color = shadowRenderer.color.WithAlpha(shadowBaseAlpha);
        
        OnRestart?.Invoke(this);
    }

    public void FadeSprite() {
        DOTween.Sequence()
            .Insert(0, spriteRenderer.DOFade(0f, 0.5f).SetEase(Ease.Linear))
            .Insert(0, shadowRenderer.DOFade(0f, 0.5f).SetEase(Ease.Linear))
            .SetTarget(spriteRenderer).SetLink(gameObject);
    }

    

    public class PlayerSnapShot
    {
        public readonly Vector3 position;
        public readonly int maxHealth;
        public readonly float ammo;

        public PlayerSnapShot (Transform playerTransform, Health playerHealth, WeaponSystem playerWeapon, Inventory inv)
        {
            position = playerTransform.position;
            maxHealth = playerHealth.GetMaxHealth();

            //Add weapon ammo and stuff here!
            ammo = playerWeapon.Ammo;
        }

        public void Apply (PlayerController pc) 
        {
            pc.transform.position = this.position;
            pc.health.SetMaxHealth(maxHealth);
            pc.health.ChangeHealth(maxHealth);
            pc.weapon.Ammo = ammo;
        }
    }
}
