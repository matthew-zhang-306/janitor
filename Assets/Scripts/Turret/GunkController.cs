using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunkController : MonoBehaviour
{
    Vector3 start_Position;
    
    // Start is called before the first frame update
    void Start()
    {

        Vector3 start_position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            //Destroy(this.gameObject);
            HideGunk();
        }
        if (other.tag == "Wall")
        {
            HideGunk();
        }
    }

    public void HideGunk()
    {
        gameObject.SetActive(false);
    }

    public void ResetGunk()
    {
        gameObject.SetActive(true);
        transform.position = start_Position;
        
    }
}
