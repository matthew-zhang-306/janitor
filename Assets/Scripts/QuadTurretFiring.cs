using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTurretFiring : BaseRoomObject
{
    public GameObject gunk;
    [SerializeField] private GameObject turretFiring;

    public static bool canShoot;
    public Vector2 Offset = new Vector2(0, 0);

    [SerializeField] private float cooldown = 2.45f;

    private bool PlayerRadius;

    private GunkController gunkcontroller;

    public bool PlayerInRange;

    GunkBulletPooler gunkPooler;

    public bool RoomTurret;
    public bool RoomActivated;
    public bool ignoreRoomStatus = false;

    //public bool shootNow;
    // Start is called before the first frame update
    void Start()
    {
        gunkPooler = GunkBulletPooler.Instance;

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
    // Update is called once per frame
    void Update()
    {

        if (!PauseMenu.GamePaused)
        {


            if (IsRoomActive || ignoreRoomStatus && canShoot)
            {
                if (RoomActivated == true)
                {
                    GameObject gunk = GunkBulletPooler.SharedInstance.GetPooledObject();
                    if (gunk != null)
                    {
                        //turret.SetTrigger("Play");
                        gunk.transform.position = turretFiring.transform.position;
                        gunk.transform.rotation = turretFiring.transform.rotation;

                        gunk.SetActive(true);

                        StartCoroutine("Delay");
                        //StartCoroutine("Cooldown");
                    }
                }
                
            }
            
        }
    }
    

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(.12f);
        GunkController.canMove = true;
    }

   
    
}
