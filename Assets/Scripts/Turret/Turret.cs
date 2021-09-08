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

    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.GamePaused)
        {
            
                if (canShoot == true)
                {
                    
                    GameObject go = (GameObject)Instantiate(gunk, (Vector2)transform.position + Offset * transform.localScale.x, Quaternion.identity);
                    go.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x * transform.localScale.x, velocity.y);

                    StartCoroutine("Cooldown");
                    


                }
            

        }
    }

    IEnumerator Cooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(3);
        
        

        canShoot = true;
    }


}
