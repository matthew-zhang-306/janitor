using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{

    public GameObject prefabBullet;
    public float firerate = 0.5f;
    public float ammoDrain = 1f;
    public float force = 1000;
    private Queue <GameObject> pool;
    public int poolCount = 100;

    protected virtual void Start () 
    {
        CreatePool();
    }

    public virtual float HandleFire (Vector3 dir, Quaternion rotation) {
        // GameObject created = GameObject.Instantiate(prefabBullet, this.transform.position, rotation);
        GameObject bullet = pool.Dequeue();
        bullet.SetActive(true);
        
        bullet.transform.position = this.transform.position;
        bullet.transform.rotation = rotation;

        var rb = bullet.GetComponent<Rigidbody2D>();
        // rb.velocity = Vector2.one * 10000 * rb.mass;
        rb.AddForce(dir * force * rb.mass);
        pool.Enqueue (bullet);
        return ammoDrain;
    }

    public virtual void CreatePool () 
    {
        var poolParent = new GameObject (prefabBullet.name + "_pool");
        pool = new Queue<GameObject>();
        for (int i = 0; i < poolCount; i++)
        {
            var created = GameObject.Instantiate(prefabBullet, poolParent.transform);
            created.SetActive (false);
            pool.Enqueue(created);
        }
    }
}