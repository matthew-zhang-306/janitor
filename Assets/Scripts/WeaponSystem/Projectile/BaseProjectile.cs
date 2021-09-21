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
    // Start is called before the first frame update
    protected virtual void OnEnable()
    {
        m_time = 0f;
        
    }
    protected virtual void Start ()
    {

    }
    // Update is called once per frame
    void Update()
    {
        m_time += Time.deltaTime;
        if (m_time > lifetime) {
            this.OnDespawn();            
            
        }
    }
    void OnDestroy()
    {
        Debug.LogWarning("bullet should not be destroyed usually");
        this.OnDespawn();

    }
    //Virtual Func for where this projectile should go
    public virtual void GetNextPosition()
    {
        
    }
    
    public virtual void OnDespawn() {
        gameObject.SetActive(false);
    }
    
}
