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

    bool cooldownShoot;
    // Start is called before the first frame update
    void Start()
    {
        turret = gameObject.GetComponent<Animator>();
        turret.SetTrigger("Play");
        GameObject go = (GameObject)Instantiate(gunk, (Vector2)transform.position + Offset * transform.localScale.x, gunk.transform.rotation);
        go.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x * transform.localScale.x, velocity.y);
        //turret_shoots = gameObject.GetComponent<Animation>();
        
        //turret_shoots.Play();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canShoot = true;
        }
        else
        {
            canShoot = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.GamePaused)
        {
            
                if (cooldownShoot == true && canShoot == true)
                {
                //  GameObject go = (GameObject)Instantiate(gunk, (Vector2)transform.position + Offset * transform.localScale.x, gunk.transform.rotation);
                // go.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x * transform.localScale.x, velocity.y);

                    gunkcontroller.ResetGunk();
                    turret.SetTrigger("Play");
                    StartCoroutine("Cooldown");

                //Quaternion.identity

            }
            
        }
    }

    
    IEnumerator Cooldown()
    {
        
        cooldownShoot = false;
        yield return new WaitForSeconds(cooldown + Random.Range(.50f, 2.0f));
        cooldownShoot = true;
    }

    


}
