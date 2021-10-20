using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTurretFiring : BaseRoomObject
{
    public GameObject gunk;

    public static bool canShoot;
    public Vector2 Offset = new Vector2(0, 0);

    [SerializeField] private float cooldown = 2.45f;

    private bool PlayerRadius;

    private GunkController gunkcontroller;

    public bool PlayerInRange;

    GunkBulletPooler gunkPooler;

    public bool ignoreRoomStatus = false;

    //public bool shootNow;
    // Start is called before the first frame update
    void Start()
    {
        gunkPooler = GunkBulletPooler.Instance;

        // if the turret is linked to a room, do not have it shoot initiallly
        if (!ignoreRoomStatus)
            canShoot = false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerInRange = true;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerInRange = false;
        }
    }


    public override void SetRoomActive(bool isActive) {
        base.SetRoomActive(isActive);

        if (isActive)
            StartCoroutine(Cooldown());
        else
            canShoot = false;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.GamePaused)
        {
            if ((IsRoomActive || (ignoreRoomStatus && PlayerInRange)) && canShoot)
            {
                GameObject gunk = GunkBulletPooler.SharedInstance.GetPooledObject();
                if (gunk != null)
                {
                    //turret.SetTrigger("Play");
                    gunk.transform.position = transform.position;
                    gunk.transform.rotation = transform.rotation;

                    gunk.SetActive(true);

                    StartCoroutine("Delay");
                    //StartCoroutine("Cooldown");
                }
            }
        }
    }
    

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(.12f);
        GunkController.canMove = true;
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }

   
    
}
