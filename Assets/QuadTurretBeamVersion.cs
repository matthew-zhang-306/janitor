using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTurretBeamVersion : BaseRoomObject
{

    public GameObject QuadTurretSprite;
    
    public bool canRotate;
    private Animator QuadAnim;
    public bool ignoreRoomStatus = false;
    //private BeamTurretFiring BeamTurret;
    public bool canShoot;
    

    // Start is called before the first frame update
    void Start()
    {
        //QuadTurretFiring.CanShootTrue(true);
        canRotate = false;
        //BeamTurret = GetComponentInChildren<BeamTurretFiring>();
        //BeamTurret.canShoot = canShoot;
        QuadAnim = QuadTurretSprite.GetComponent<Animator>();

        StartCoroutine("Cooldown");

        if ((IsRoomActive || ignoreRoomStatus))
        {
            canShoot = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.GamePaused)
        {
            //BeamTurret.canShoot = canShoot;

            if (canRotate)
            {
            transform.Rotate(0, 0, .15f);
            }
            
        }

    }

    public override void SetRoomActive(bool isActive)
    {
        base.SetRoomActive(isActive);

        if (isActive)
            StartCoroutine(Cooldown());
        else
            canShoot = false;
    }

    IEnumerator Firing()
    {
        canShoot = true;
        QuadAnim.SetTrigger("Firing");
        //SoundManager.PlaySound(SoundManager.Sound.Quad, 0.5f);
        canRotate = true;
        yield return new WaitForSeconds(5);
        StartCoroutine("Cooldown");
    }
    IEnumerator Cooldown()
    {
        QuadAnim.SetTrigger("Reload");
        canShoot = false;
        canRotate = false;
        yield return new WaitForSeconds(3);
        StartCoroutine("Firing");


    }
}
