using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{

    public GameObject gunk;
    public Vector2 velocity;

    public bool canShoot = true;
    public Vector2 Offset = new Vector2(0.4f, 0.1f);

    public float cooldown = 1f;

    private bool PlayerRadius;

    //private Animation turret_shoots;
    private Animator turret;

    private GunkController gunkcontroller;

    public bool PlayerInRange;

    GunkBulletPooler gunkPooler;

    public bool RoomTurret;
    public  bool RoomActivated;

    //public bool shootNow;
    // Start is called before the first frame update
    void Start()
    {
        gunkPooler = GunkBulletPooler.Instance;
        turret = gameObject.GetComponent<Animator>();
        RoomActivated = false;
         
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

    void Update()
    {
        if (!PauseMenu.GamePaused)
        {
            if (RoomTurret == true)
            {
                if (RoomActivated == true && canShoot == true)
                {
                    GameObject gunk = GunkBulletPooler.SharedInstance.GetPooledObject();
                    if (gunk != null)
                    {
                        turret.SetTrigger("Play");
                        gunk.transform.position = turret.transform.position;
                        gunk.transform.rotation = turret.transform.rotation;

                        gunk.SetActive(true);

                        StartCoroutine("Delay");
                        StartCoroutine("Cooldown");
                    }
                }
            }
            if (RoomTurret == false)
            {
                if (PlayerInRange == true && canShoot == true)
                {
                    GameObject gunk = GunkBulletPooler.SharedInstance.GetPooledObject();
                    if (gunk != null)
                    {
                        turret.SetTrigger("Play");
                        gunk.transform.position = turret.transform.position;
                        gunk.transform.rotation = turret.transform.rotation;

                        gunk.SetActive(true);

                        StartCoroutine("Delay");
                        StartCoroutine("Cooldown");
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

    IEnumerator Cooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(cooldown + Random.Range(.15f, .4f));
        canShoot = true;
    }

}
