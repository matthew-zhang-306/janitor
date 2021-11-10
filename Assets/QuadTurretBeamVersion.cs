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
        canRotate = false;
        QuadAnim = QuadTurretSprite.GetComponent<Animator>();

        

        if ((IsRoomActive || ignoreRoomStatus))
        {
            StartCoroutine("Cooldown");
        }
        else
        {
            canShoot = false;
            canRotate = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.GamePaused && (IsRoomActive || ignoreRoomStatus))
        {
            //BeamTurret.canShoot = canShoot;

            if (canRotate)
            {
            transform.Rotate(0, 0, .15f);
            }
            
        }
        else
        {
            canShoot = false;
            canRotate = false;
            StopAllCoroutines();
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
