using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private int currentHealth = 1;
    
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void ChangeHealth (int delta) 
    {   
        currentHealth += delta;
        
        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
        
    }

    public int GetHealth ()
    {
        return currentHealth;
    }

    public float GetHealthPercent()
    {
        return (float)currentHealth / (float)maxHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetMaxHealth(int newHealth)
    {
        //honestly should use autos but oh well
        maxHealth = newHealth;
        if (maxHealth < 1) {
            maxHealth = 1;
        }
    }
    public void AddHealth()
    {
        Debug.Log("Add player Health");
        currentHealth = currentHealth + 10;
    }
}
