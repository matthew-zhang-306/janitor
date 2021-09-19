﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunkController : MonoBehaviour
{
    
    [SerializeField] private GameObject gunk;
    [SerializeField] Vector2 velocity;

    //GunkBulletPooler gunkPooler;

    // Start is called before the first frame update
    void Start()
    {

        //gunk.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x * transform.localScale.x, velocity.y);


    }

    // Update is called once per frame
    void Update()
    {
        gunk.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x * transform.localScale.x, velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            this.gameObject.SetActive(false);
        }
        if (other.tag == "Wall")
        {
            this.gameObject.SetActive(false);
        }
    }

    
}