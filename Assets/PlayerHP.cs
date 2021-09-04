using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    GameObject Player;
    public Slider healthBar;
    // Start is called before the first frame update
    void Start()
    {
        Player.GetComponent<HealthAddon>();
    }

    // Update is called once per frame
    void Update()
    {
       // healthBar.maxValue = HealthAddon.maxHealth;
       // healthBar.value = HealthAddon.currentHealth;
    }

}
