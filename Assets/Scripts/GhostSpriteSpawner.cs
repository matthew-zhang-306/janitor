using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSpriteSpawner : MonoBehaviour
{
    //[Replace with a pooling system to make the game faster]
    [SerializeField] GameObject spritePrefab;

    [SerializeField] SpriteRenderer spriteRenderer;

    //Time until spawning the next Ghost
    [SerializeField] float time = 0.2f;
    float currentTime = 0;
    Queue<GameObject> pool;

    

    void Awake()
    {
        CreatePool();

    }

    //Called from the Update function in the movement Class either Player or Enemy
    public void Update()
    {
        //Add the time amount each Update
        currentTime += Time.deltaTime;

        //When the time is reach
        if (currentTime >= time)
        {
            StartCoroutine(SpawnGhost());
            //Reset Time
            currentTime = 0;
        }
    }

    //Put a ghost on the scene
    IEnumerator SpawnGhost()
    {
        //Start a new Ghost [Replace with a pooling system to make the game faster]
        GameObject ghost = pool.Dequeue();
        pool.Enqueue(ghost);
        ghost.SetActive(true);

        ghost.GetComponent<Animator>().SetTrigger("Dash");
        

        //Get the same sprite and flip direction 
        ghost.GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
        ghost.GetComponent<SpriteRenderer>().flipX = spriteRenderer.flipX;
        ghost.transform.position = transform.position;

        //Destroy the Ghost after a while [Replace with a pooling system to make the game faster]
        yield return new WaitForSeconds(2f);
        ghost.SetActive(false);
    }
    public virtual void CreatePool()
    {
        var poolParent = new GameObject(spritePrefab.name + "_pool");
        pool = new Queue<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            var created = GameObject.Instantiate(spritePrefab, poolParent.transform);
            created.SetActive(false);
            pool.Enqueue(created);
        }
    }
    }
