using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSpriteSpawner : MonoBehaviour
{
    [SerializeField] GameObject spritePrefab;
    [SerializeField] SpriteRenderer playerSpriteRenderer;

    //Time until spawning the next Ghost
    [SerializeField] float spawnTime = 0.2f;
    Queue<GameObject> pool;

    

    void Awake()
    {
        CreatePool();
    }

    private void OnEnable() {
        PlayerController.OnDash += OnDash;
    }
    private void OnDisable() {
        PlayerController.OnDash -= OnDash;
    }


    public void OnDash(PlayerController player)
    {
        if (player.gameObject != this.gameObject) {
            return;
        }

        StartCoroutine(DoDash(player.DashTime));
    }

    public IEnumerator DoDash(float dashTime)
    {
        float dashTimer = 0;
        float spawnTimer = spawnTime;

        while (dashTimer < dashTime) {
            dashTimer += Time.deltaTime;
            spawnTimer += Time.deltaTime;

            while (spawnTimer >= spawnTime)
            {
                spawnTimer -= spawnTime;
                StartCoroutine(SpawnGhost());
            }

            yield return 0;
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
        ghost.GetComponent<SpriteRenderer>().sprite = playerSpriteRenderer.sprite;
        ghost.GetComponent<SpriteRenderer>().flipX = playerSpriteRenderer.flipX;
        ghost.transform.position = playerSpriteRenderer.transform.position;

        //Destroy the Ghost after a while [Replace with a pooling system to make the game faster]
        yield return new WaitForSeconds(0.5f);
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
