using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponSystem : MonoBehaviour
{
    public Camera cam;

    //Change later (move to weapon)
    public float meleerate = 0.2f;
    private float m_ftime = 0f;

    private float _maxAmmo = 100f;
    public float MaxAmmo 
    {
        get => _maxAmmo;
        private set {}
    }

    public float _ammo;

    public float Ammo
    {
        get => _ammo;
        set {
            _ammo = Mathf.Clamp(value, 0, MaxAmmo);
        }
    }

    [SerializeField] private float ammoRestorationScale = 0.5f;

    public GameObject bulletPrefab;
    public GameObject meleePrefab;
    //public AudioSource gunSound;
    //public AudioSource meleeSound;
   // public AudioClip[] meleeEffects;

    public BaseWeapon weapon;
    // Start is called before the first frame update
    void Start()
    {
        //Ammo = _maxAmmo;
        Ammo = 0;
       // meleeSound = meleeSE.GetComponent<AudioClip>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log (Input.mousePosition);
        // Debug.Log (CustomInput.GetMousePosition());

        #if (UNITY_ANDROID || UNITY_IPHONE)
        Vector2 dir = new Vector2 (CustomInput.axis2x, CustomInput.axis2y).normalized;

        #else 
        Vector3 hit = cam.ScreenToWorldPoint(Input.mousePosition);
        Debug.DrawLine (hit, transform.position);
        Vector2 dir = (hit - transform.position).ToVector2().normalized;
        #endif

        
        this.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir));
        //well we should probs consider top and bottom rotations too

        this.transform.localScale = new Vector3 (1, Mathf.Sign (dir.x), 1);

        if (m_ftime > weapon.firerate
                #if (UNITY_ANDROID || UNITY_IPHONE)
                && CustomInput.ranged
                && dir.magnitude != 0
                #else
                && Input.GetButton("Fire1") 
                #endif
            ) {
            Vector3 mousePos = CustomInput.GetMousePosition();

            Fire (dir);
            //gunSound.Play();
            SoundManager.PlaySound(SoundManager.Sound.spongeGun, 0.5f);
            m_ftime = 0;
        }
        if (m_ftime > meleerate  
                #if (UNITY_ANDROID || UNITY_IPHONE)
                && CustomInput.melee
                && dir.magnitude != 0
                #else
                && Input.GetButton("Fire2")                
                #endif
            ) {
            Vector3 mousePos = CustomInput.GetMousePosition();

            Swing (dir);
            //meleeSound.clip = meleeEffects[Random.Range(0, meleeEffects.Length)];
            //meleeSound.Play();
            SoundManager.PlaySound(SoundManager.Sound.Broom, SettingsMenu.SEvolume);
            m_ftime = 0;
        }
        m_ftime += Time.deltaTime;
    }

    public void Fire (Vector3 dir) {
        
        
        Quaternion bulletRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir));

        if (this.weapon.CanFire(_ammo))
        {
            var drain = this.weapon.HandleFire(dir, bulletRotation);
            _ammo -= drain;
        }

        //Maybe change to Entity System Later
        
        
        // Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), created.GetComponent<Collider2D>());
    }


    public void Swing (Vector3 dir) {
        

        //YA know I have no clue why exactly these value work sooo
        //BUT particle render must be set to LOCAL instead of view for rotation lol
        Quaternion bulletRotation = Quaternion.Euler(Vector2.SignedAngle(Vector2.right, dir) - 18.5f, -90, -90);

        GameObject created = GameObject.Instantiate(meleePrefab, this.transform.position + dir, bulletRotation);
        var fm = created.GetComponent<FloorMarker>();
        if (fm == null)
        {
            Debug.LogError("Melee prefab does not have a floor marker");
            fm = created.AddComponent<TemporaryFloorMarker>();
        }
            

        fm.callback += (value) => {
            Ammo += value * ammoRestorationScale;
        };
    }
}
