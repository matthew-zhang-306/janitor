using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BaseProjectile : MonoBehaviour
{
    public float lifetime = 3f;
    private float m_time = 0f;
    private Tilemap tm;
    public Tile tile;
    // Start is called before the first frame update
    void Start()
    {
        m_time = 0f;
        tm = GameObject.Find("/Grid").transform.GetChild(0).GetComponent<Tilemap>();
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
            Vector3Int pos = tm.WorldToCell(this.transform.position);
            TileBase c_tile = tm.GetTile(pos);
            if (c_tile != null) {
                tm.SetTile(pos, tile);
            }
        }
    }

    //Virtual Func for where this projectile should go
    public virtual void GetNextPosition()
    {
        
    }
    
    public virtual void OnDespawn() {

    }
}
