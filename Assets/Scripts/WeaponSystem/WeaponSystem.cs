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
    //public AudioSource gunSound;
    //public AudioSource meleeSound;
   // public AudioClip[] meleeEffects;

    public BaseWeapon weapon;
    // Start is called before the first frame update
    void Start()
    {
       // meleeSound = meleeSE.GetComponent<AudioClip>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 hit = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (hit - transform.position).ToVector2().normalized;
        this.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir));
        //well we should probs consider top and bottom rotations too

        this.transform.localScale = new Vector3 (1, Mathf.Sign (dir.x), 1);

        if (Input.GetButton("Fire1") && m_ftime > weapon.firerate) {
            Vector3 mousePos = Input.mousePosition;

            Fire (mousePos);
            //gunSound.Play();
            SoundManager.PlaySound(SoundManager.Sound.spongeGun, SettingsMenu.SEvolume);
            m_ftime = 0;
        }
        if (Input.GetButton("Fire2") && m_ftime > meleerate) {
            Vector3 mousePos = Input.mousePosition;

            Swing (mousePos);
            //meleeSound.clip = meleeEffects[Random.Range(0, meleeEffects.Length)];
            //meleeSound.Play();
            SoundManager.PlaySound(SoundManager.Sound.Broom, SettingsMenu.SEvolume);
            m_ftime = 0;
        }
        m_ftime += Time.deltaTime;
    }

    public void Fire (Vector3 p) {
        
        Vector3 hit = cam.ScreenToWorldPoint(p);
        Vector2 dir = (hit - transform.position).ToVector2().normalized;
        Quaternion bulletRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir));

        this.weapon.HandleFire(dir, bulletRotation);


        //Maybe change to Entity System Later
        
        
        // Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), created.GetComponent<Collider2D>());
    }


    public void Swing (Vector3 p) {
        Vector3 hit = cam.ScreenToWorldPoint(p);
        Vector2 dir = (hit - transform.position).ToVector2().normalized;
        Quaternion bulletRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, dir));

        GameObject created = GameObject.Instantiate(meleePrefab, this.transform.position, bulletRotation);
    }
}
