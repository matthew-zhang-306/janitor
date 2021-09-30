using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private new Collider2D collider;
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
    [SerializeField] private float dashSpeed = 50f;

    [Header("Getting Hit")]
    [SerializeField] private float knockbackPower = 1f;
    [SerializeField] private float knockbackTime = 1f;
    [SerializeField] private float knockbackFriction = 1f;
    [SerializeField] private float invincibilityTime = 1f;

    

    private void Start() {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
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
        dashTimer = Mathf.MoveTowards(dashTimer, 0f, Time.deltaTime);

        if (dashTimer <= dashCooldown - dashTime) {
            HandleMotion();
        }

        invincibilityTimer = Mathf.MoveTowards(invincibilityTimer, 0, Time.deltaTime);
    }

    private void OnEnterHazard(Collider2D other) {
        other.GetComponentInParent<BaseProjectile>()?.OnHitEntity();
            
        if (invincibilityTimer == 0f) {
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
        velocity = Vector2.MoveTowards(velocity, moveInput * (maxSpeed), acceleration);
        rb2d.velocity = velocity;
        
        // handle dashing
        bool isDash = Input.GetButton("Jump");
        if (isDash && dashTimer <= 0f) {
            StartCoroutine(DoDash(moveInput));
        }
    }

    private IEnumerator DoDash(Vector2 dashDirection) {
        if (dashDirection == Vector2.zero) {
            // TODO: use the player's "facing direction" here
            yield break;
        }

        Vector3 dashPoint = CalculateDashPoint(dashDirection, out Vector3 lastHolePosition);

        bool collisionDisabled = false;
        bool inLastHole = collider.bounds.Contains(lastHolePosition);
        bool oldInLastHole = inLastHole;
        if (!inLastHole) {
            // this dash will cross over holes
            collisionDisabled = true;
            collider.isTrigger = true;
        }

        // ignore enemy hits for a while
        // TODO: add a different dash iframe timer
        invincibilityTimer += dashIframe;

        dashTimer = dashCooldown;
        while (dashTimer > dashCooldown - dashTime) {
            oldInLastHole = inLastHole;
            inLastHole = collider.bounds.Contains(lastHolePosition);

            if (collisionDisabled && (!inLastHole && oldInLastHole)) {
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

        lastHolePosition = collider.bounds.center;

        List<RaycastHit2D> forwardHits = Physics2D.RaycastAll(
            collider.bounds.center,
            dashDirection,
            dashSpeed * dashTime,
            LayerMask.GetMask("Wall", "Sides", "Hole")
        ).ToList();
        if (forwardHits.Count > 0) {
            // do not consider anything past the first wall
            int firstWallHit = forwardHits.FindIndex(
                hit => hit.transform.gameObject.layer != LayerMask.NameToLayer("Hole")
            );
            if (firstWallHit != -1) {
                desiredDashPoint = forwardHits[firstWallHit].point;
                forwardHits = forwardHits.GetRange(0, firstWallHit);
            }

            if (forwardHits.Count > 0) {
                // find the furthest reachable ground by raycasting backward
                List<RaycastHit2D> backwardHits = Physics2D.RaycastAll(
                    desiredDashPoint,
                    -dashDirection,
                    Vector2.Distance(collider.bounds.center, desiredDashPoint),
                    LayerMask.GetMask("Hole")
                ).ToList();

                if (forwardHits.Count != backwardHits.Count) {
                    Debug.LogError("When determining dash distance, the player had " + forwardHits.Count + " forward raycast hits but " + backwardHits.Count + " backward raycast hits, which are different numbers. This is not expected behavior so you should ask Matt about it if it happens because it could be something weird");
                }

                int f = forwardHits.Count;
                int b = 0;
                while (b < backwardHits.Count) {
                    var backwardHitPos = backwardHits[b].point;
                    var forwardHitPos = f < forwardHits.Count ? forwardHits[f].point : desiredDashPoint;

                    if (Vector2.Distance(backwardHitPos, forwardHitPos) > 0.1f) {
                        // it looks like we can dash here. keep this f value by exiting the loop
                        break;
                    }

                    b++;
                    f--;
                }
                desiredDashPoint = f < forwardHits.Count ? forwardHits[f].point : desiredDashPoint;
                if (b < backwardHits.Count)
                    lastHolePosition = backwardHits[b].point;

                // debugging lines
                Debug.DrawRay(collider.bounds.center, dashDirection.ToVector3() * dashSpeed * dashTime, Color.white, 5f);
                for (var a = 0; a < forwardHits.Count; a++) {
                    Debug.DrawLine(collider.bounds.center + transform.up * (a+1), forwardHits[a].point, Color.magenta, 5f);
                }
                for (var a = 0; a < backwardHits.Count; a++) {
                    Debug.DrawLine(collider.bounds.center - transform.up * (a+1), backwardHits[a].point, Color.red, 5f);
                }
                Debug.DrawLine(collider.bounds.center, desiredDashPoint, Color.blue, 5f);
                Debug.DrawLine(collider.bounds.center, lastHolePosition, Color.cyan, 5f);
            }
        }

        return desiredDashPoint.ToVector3();
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
