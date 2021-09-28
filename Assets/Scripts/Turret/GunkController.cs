using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunkController : MonoBehaviour
{
    
    [SerializeField] private GameObject gunk;
    //[SerializeField] Vector2 velocity;
    public static bool canMove;
    [SerializeField] private float force = 10f;
    //GunkBulletPooler gunkPooler;
    
    // Start is called before the first frame update
    void Start()
    {

        //gunk.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x * transform.localScale.x, velocity.y);
     //   canMove = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        Debug.Log("can move " + canMove);
        if (canMove == true)
        {
            //gunk.SetActive(true);
            gunk.GetComponent<Rigidbody2D>();
            gunk.transform.position += transform.up * force * Time.deltaTime;

            //gunk.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x * transform.localScale.x, velocity.y);

        }
        
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
        //    canMove = false;
            this.gameObject.SetActive(false);
        }
        if (other.tag == "Wall")
        {
          //  canMove = false;
            this.gameObject.SetActive(false);
        }
        if (other.tag == "Enemy")
        {
          //  canMove = false;
           // this.gameObject.SetActive(false);
        }
    }

    
}
