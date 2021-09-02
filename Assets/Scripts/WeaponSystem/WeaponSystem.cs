using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponSystem : MonoBehaviour
{
    public Camera cam;

    //Change later (move to weapon)
    public float firerate = 0.5f;
    private float m_ftime = 0f;

    public GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && m_ftime > firerate) {
            Vector3 mousePos = Input.mousePosition;

            Fire (mousePos);
            m_ftime = 0;
        }
        m_ftime += Time.deltaTime;
    }

    public void Fire (Vector3 p) {
        
        Vector3 hit = cam.ScreenToWorldPoint(p);

        Vector3 dir3 = hit - transform.position;
        
        Vector2 dir = new Vector2(dir3.x, dir3.y);
        dir.Normalize();
        // Debug.Log(dir);

        //Maybe change to Entity System Later
        GameObject created = GameObject.Instantiate(prefab, this.transform.position, Quaternion.identity);
        created.transform.LookAt(transform.position + dir3 * 300, Vector3.forward);
        created.SetActive(true);
        var rb = created.GetComponent<Rigidbody2D>();
        rb.AddForce(dir * 1000 * rb.mass);
        
        // Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), created.GetComponent<Collider2D>());
    }
}
