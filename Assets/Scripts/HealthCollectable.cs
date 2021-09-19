using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectable : MonoBehaviour
{
    private GameObject Player;
    private bool TouchedPlayer;
    private Health health;
    private int currentHealth;
    private int maxHealth;
    // Start is called before the first frame update
    void Start()
    {
        GameObject Player = GameObject.Find("Player");
        health = Player.GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = health.GetHealth();
        maxHealth = health.GetMaxHealth();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && (currentHealth<maxHealth))
        {
            
            health.AddHealth();
            Destroy(this.gameObject);
        }
    }

    
}
