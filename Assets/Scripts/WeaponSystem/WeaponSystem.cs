using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponSystem : MonoBehaviour
{
    public Camera cam;

    //Change later (move to weapon)
    public float firerate = 0.5f;
    public float meleerate = 0.2f;
    private float m_ftime = 0f;

    public GameObject bulletPrefab;
    public GameObject meleePrefab;
    public AudioSource gunSound;
    public AudioSource meleeSound;
    public AudioClip[] meleeEffects;
    // Start is called before the first frame update
    void Start()
    {
       // meleeSound = meleeSE.GetComponent<AudioClip>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && m_ftime > firerate) {
            Vector3 mousePos = Input.mousePosition;

            Fire (mousePos);
            gunSound.Play();
            m_ftime = 0;
        }
        if (Input.GetButton("Fire2") && m_ftime > meleerate) {
            Vector3 mousePos = Input.mousePosition;

            Swing (mousePos);
            meleeSound.clip = meleeEffects[Random.Range(0, meleeEffects.Length)];
            meleeSound.Play();
            m_ftime = 0;
        }
        m_ftime += Time.deltaTime;
    }

    public void Fire (Vector3 p) {
        
        Vector3 hit = cam.ScreenToWorldPoint(p);
        Vector2 dir = (hit - transform.position).ToVector2().normalized;
        Quaternion bulletRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, dir));

        //Maybe change to Entity System Later
        GameObject created = GameObject.Instantiate(bulletPrefab, this.transform.position, bulletRotation);
        var rb = created.GetComponent<Rigidbody2D>();
        rb.AddForce(dir * 1000 * rb.mass);
        
        // Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), created.GetComponent<Collider2D>());
    }


    public void Swing (Vector3 p) {
        Vector3 hit = cam.ScreenToWorldPoint(p);
        Vector2 dir = (hit - transform.position).ToVector2().normalized;
        Quaternion bulletRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, dir));

        GameObject created = GameObject.Instantiate(meleePrefab, this.transform.position, bulletRotation);
    }
}
