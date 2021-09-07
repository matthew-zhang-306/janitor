using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class DoorsOpen : MonoBehaviour
{

    
    public TilemapRenderer doorsR;
    public TilemapCollider2D doorsC;
    public GameObject testroom;
    // Start is called before the first frame update
    void Start()
    {
        
        
        
        
        doorsC.enabled = false;
        doorsR.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            
            testroom.SetActive(true);
            doorsC.enabled = true;
            doorsR.enabled = true;
        }
    }
}
