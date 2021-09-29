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

        
        
    }

    // Update is called once per frame
    void Update()
    {
        
        Debug.Log("can move " + canMove);
        if (canMove == true)
        {
            
            gunk.GetComponent<Rigidbody2D>();
            gunk.transform.position += transform.up * force * Time.deltaTime;

            

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
