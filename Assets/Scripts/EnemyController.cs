using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyController : MonoBehaviour
{
    private Tilemap tm;
    public Tile tile;

    public Hitbox hitbox;
    public SpriteRenderer spriteRenderer;

    private Health health;
    private Rigidbody2D rb2d;
    public GameObject floorMarker;

    public float agroRadius;
    [HideInInspector] public PlayerController player;

    [SerializeField] private float actionTimer = 1.5f;
    private float m_time;

    [SerializeField] private float invincibilityTime = 0.2f;
    private float invincibilityTimer;

    [SerializeField] private float dirtyTime = 1f;

    private string[] actions;

    private UnityEvent deathEvent;
    
    // Start is called before the first frame update
    void Start()
    {
        //note an enemy does not 'have' to have health so health may be null.
        health = this.GetComponent<Health>();
        rb2d = GetComponent<Rigidbody2D>();

        m_time = 0f;

        DOTween.Sequence()
            .InsertCallback(0f, () => floorMarker.SetActive(true))
            .InsertCallback(0.2f, () => floorMarker.SetActive(false))
            .InsertCallback(dirtyTime, () => floorMarker.SetActive(true))
            .SetLoops(-1, LoopType.Restart)
            .SetLink(gameObject);
    }  
    void Awake() {
        if (deathEvent == null)
            deathEvent = new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_time > actionTimer) {
            m_time -= actionTimer;

            // chose action
            if (player != null &&
                Vector3.Distance(player.transform.position, this.transform.position) <= agroRadius)
            {
                Action_Move();
            } else {
                Action_Wander();
            }
        }
        m_time += Time.deltaTime;
    }

    private void FixedUpdate() {
        spriteRenderer.color = invincibilityTimer > 0 ? new Color(1, 0.4f, 0.4f) : Color.white;

        invincibilityTimer = Mathf.MoveTowards(invincibilityTimer, 0f, Time.deltaTime);
        if (health != null && hitbox.IsColliding && invincibilityTimer == 0f) {
            // take a hit
            TakeDamage(hitbox.OtherCollider);
        }
    }

    void OnDestroy()
    {
        Debug.Log("Destroying obj");
        deathEvent.Invoke();
        deathEvent.RemoveAllListeners();
    }

    //Action Methods should only be called from itself
    protected virtual void Action_Wander ()
    {
        Vector3 randomDir = Quaternion.Euler(0, 0, Random.Range(0f, 360f)) * Vector3.right;
        rb2d.AddForce(randomDir * 30 * rb2d.mass);
    }

    protected virtual void Action_Move ()
    {
        Vector3 dir = player.transform.position - this.transform.position;
        // randomize direction a little bit
        dir = Quaternion.Euler(0, 0, Random.Range(-15f, 15f)) * dir;

        rb2d.AddForce(dir * 40 * rb2d.mass);
    }
    
    void OnCollisionEnter2D (Collision2D col) {
        if (health != null && col.gameObject.tag == "PlayerProjectile" && invincibilityTimer == 0f) {
            TakeDamage(col.collider);
        }
    }


    private void TakeDamage(Collider2D other) {
        int hitAmount = 1;
        var damage = other.GetComponent<Damage>();
        if (damage != null) {
            hitAmount = damage.damage;
        }
        health.ChangeHealth(-hitAmount);
        
        float per = (1 - health.GetHealthPercent())/2 + health.GetHealthPercent();
        this.transform.localScale = Vector3.one * per;

        invincibilityTimer = invincibilityTime;
        Destroy(other.gameObject);

        if (health.GetHealth() <= 0) {
            Destroy(gameObject);
        }
    }

    //Please change this to normal events later thx
    //unityevent be really inefficient
    public UnityEvent GetDeathEvent () 
    {
        return deathEvent;
    }

    
    
}