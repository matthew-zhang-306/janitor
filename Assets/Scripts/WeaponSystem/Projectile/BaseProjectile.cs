using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BaseProjectile : MonoBehaviour
{
    public float lifetime = 1f;
    private float m_time = 0f;
    private Tilemap tm;
    public Tile tile;
    // Start is called before the first frame update
    void Start()
    {
        m_time = 0f;
        
    }

    // Update is called once per frame
    void Update()
    {
        m_time += Time.deltaTime;
        if (m_time > lifetime) {
            this.OnDespawn();            
            Destroy(gameObject);
        }
        else {
            
        }
    }
    void OnDestroy()
    {
        this.OnDespawn();
    }
    //Virtual Func for where this projectile should go
    public virtual void GetNextPosition()
    {
        
    }
    
    public virtual void OnDespawn() {

    }

    void OnTriggerStay2D (Collider2D col) {
        // Debug.Log (col.gameObject.name);
        // if (col.gameObject.tag == "PlayerProjectile") {
        //     if (health != null) {
        //         health.ChangeHealth(-1);
        //         float per = (1 - health.GetHealthPercent())/2 + health.GetHealthPercent();
        //         this.transform.localScale = new Vector3(per,per,per);
        //     }
        //     Destroy(col.gameObject);
        // }
        // Debug.Log("hi there");

        // if (health.GetHealth() <= 0) {
        //     Destroy(gameObject);
        // }
    }
    
}
