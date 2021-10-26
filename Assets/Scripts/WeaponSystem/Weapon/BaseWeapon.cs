using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : Upgradeable
{

    public GameObject prefabBullet;
    [SerializeField] public float firerate = 0.5f;
    [SerializeField] protected float ammoDrain = 1f;
    [SerializeField] protected float force = 1000;
    [SerializeField] protected float bulletDamage = 5;

    private Queue <GameObject> pool;
    public int poolCount = 100;

    protected virtual void Start () 
    {
        CreatePool();
        
    }

    public virtual bool CanFire (float currentAmmo)
    {
        return ammoDrain <= currentAmmo;
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

    public virtual float HandleFire (Vector3 dir, Quaternion rotation, out GameObject bulletGameObject) {
        // GameObject created = GameObject.Instantiate(prefabBullet, this.transform.position, rotation);
        GameObject bullet = pool.Dequeue();
        bulletGameObject = bullet;
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