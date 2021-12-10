using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponSystem : Upgradeable
{
    public static EmptyDelegate OnWeaponFire;

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

    [SerializeField] private bool ZeroAmmoStart;

    [Header("1 for 100% snap to target, 0 for none. ")]
    private float aimAssist; 
    [HideInInspector] public Transform target;
    
    private Vector2 mouseBefore;
    private Vector2 prevControllerDir;
    bool controllerMode;
    public BaseWeapon weapon;
    // Start is called before the first frame update
    void Start()
    {
        GetBaseProps();
        Ammo = _maxAmmo;
        if (ZeroAmmoStart)
        {
            Ammo = 0;
        }
        controllerMode = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.deltaTime == 0) {
            return;
        }
    
        Vector2 dir = new Vector2 (CustomInput.axis2x, CustomInput.axis2y).normalized;

        if (dir.magnitude == 0) {
            if (!controllerMode || mouseBefore != CustomInput.GetMousePosition()) {
                Vector3 hit = cam.ScreenToWorldPoint(CustomInput.GetMousePosition());
                Debug.DrawLine (hit, transform.position);
                dir = (hit - transform.position).ToVector2().normalized;
                aimAssist = 0;
                mouseBefore = CustomInput.GetMousePosition();
                controllerMode = false;
            }
            else {
                dir = prevControllerDir;
            }
        }
        else {
            prevControllerDir = dir;
            controllerMode = true;
            aimAssist = 0.3f;
        }
        if (dir.magnitude == 0) {
            dir = Vector2.left;
        }
        Vector2 odir = dir;

        //Aim assist here
        //Get distance and angle from player to each enemy
        if (target) {

            //Priorty to Closest Enemy
            var best = (dir, 1000f);

            foreach (Transform enemy in target) {
                var dist = Vector2.Distance(enemy.position, transform.position);
                var angle = Vector2.Angle(dir, enemy.position - transform.position);
                if (best.Item2 > dist && Mathf.Deg2Rad * angle < aimAssist) {
                    best = ((enemy.position - transform.position).ToVector2(), dist);
                }

            }
            dir = best.Item1;
        }
        dir.Normalize();
        
        this.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir));
        //well we should probs consider top and bottom rotations too

        this.transform.localScale = new Vector3 (1, Mathf.Sign (dir.x), 1);

        if (m_ftime > weapon.firerate
                #if (UNITY_ANDROID || UNITY_IPHONE)
                && CustomInput.ranged
                && odir.magnitude != 0
                #else
                && CustomInput.GetButton("Fire1") 
                #endif
            ) {

            Fire (dir);
            
            m_ftime = 0;
        }
        if (m_ftime > meleerate  
                #if (UNITY_ANDROID || UNITY_IPHONE)
                && CustomInput.melee
                && odir.magnitude != 0
                #else
                && CustomInput.GetButton("Fire2")                
                #endif
            ) {

            Swing (dir);
            m_ftime = 0;
        }
        m_ftime += Time.deltaTime;

        //check for ammo and if ammo is low flash the low ammo UI above player and make the ammo bar blink
        if (_ammo < 10)
        {
            AmmoOnboarding.AmmoLow = true;
           // SoundManager.PlaySound(SoundManager.Sound.AmmoAlert, 0.4f);
        }
        if (_ammo > 10)
        {
            AmmoOnboarding.AmmoLow = false;
        }

    }

    public void Fire (Vector3 dir) {
        
        
        Quaternion bulletRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir));

        if (this.weapon.CanFire(_ammo))
        {
            var drain = this.weapon.HandleFire(dir, bulletRotation);
            _ammo -= drain;
            //gun fire sound here
            SoundManager.PlaySound(SoundManager.Sound.spongeGun, 0.5f);
            OnWeaponFire?.Invoke();
        }
        else
        {
            //play the can't fire sound
            //put the can't fire sound connection here boram
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
        SoundManager.PlaySoundBuffered(SoundManager.Sound.Broom1, 0.4f, meleerate + 0.01f);

    }
}
