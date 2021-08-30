using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    public float lifetime = 3f;
    private float m_time = 0f;
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

    //Virtual Func for where this projectile should go
    public virtual void GetNextPosition()
    {
        
    }
    
    public virtual void OnDespawn() {

    }
}
