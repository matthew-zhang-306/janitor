using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyController : MonoBehaviour
{
    private Tilemap tm;
    public Tile tile;

    private Hitbox hitbox;
    private Health health;
    private Rigidbody2D rb2d;
    public GameObject floorMarker;

    [SerializeField] private float actionTimer = 1.5f;
    private float m_time;

    [SerializeField] private float invincibilityTime = 0.2f;
    private float invincibilityTimer;

    [SerializeField] private float dirtyTime = 1f;

    private string[] actions;
    // Start is called before the first frame update
    void Start()
    {
        //note an enemy does not 'have' to have health so health may be null.
        hitbox = GetComponentInChildren<Hitbox>();
        health = this.GetComponent<Health>();
        rb2d = GetComponent<Rigidbody2D>();
        // tm = GameObject.Find("/Grid").transform.GetChild(0).GetComponent<Tilemap>();

        m_time = 0f;
        actions = new string[]{"Move", "Stay"};

        DOTween.Sequence()
            .InsertCallback(0f, () => floorMarker.SetActive(true))
            .InsertCallback(0.2f, () => floorMarker.SetActive(false))
            .InsertCallback(dirtyTime, () => floorMarker.SetActive(true))
            .SetLoops(-1, LoopType.Restart)
            .SetLink(gameObject);
    }  
 

    // Update is called once per frame
    void Update()
    {
        if (m_time > actionTimer) {
            m_time -= actionTimer;
            //chose action
            int rand = (int)UnityEngine.Random.Range(0, actions.Length);
            Invoke("Action_" + actions[rand], 0f);
        }
        m_time += Time.deltaTime;

        //Put this somewhere else after proto
        // Vector3Int pos = tm.WorldToCell(this.transform.position);
        // TileBase c_tile = tm.GetTile(pos);
        // if (c_tile != null) {
        //     tm.SetTile(pos, tile);
        // }
    }

    private void FixedUpdate() {
        GetComponent<SpriteRenderer>().color = invincibilityTimer > 0 ? new Color(1, 0.4f, 0.4f) : Color.white;

        invincibilityTimer = Mathf.MoveTowards(invincibilityTimer, 0f, Time.deltaTime);
        if (health != null && hitbox.IsColliding && invincibilityTimer == 0f) {
            // take a hit
            TakeDamage(hitbox.OtherCollider);
        }
    }

    //Action Methods should only be called from itself
    protected virtual void Action_Stay ()
    {
        // Debug.Log("doing nothing");
        //do nothing
        m_time += 0.5f;
        return;
    }
    protected virtual void Action_Move ()
    {
        //Get closest 'player'
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            // Debug.Log("no player detected");
            //no player detected
            return;
        }

        Vector3 dir = player.transform.position - this.transform.position;
        rb2d.AddForce(dir * 40 * rb2d.mass);
        //do nothing
        return;
    }
    
    void OnCollisionEnter2D (Collision2D col) {
        if (health != null && col.gameObject.tag == "PlayerProjectile") {
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
}
