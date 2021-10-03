using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{

    public GameObject Player;
    public Slider ammoBar;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = Player.GetComponent<PlayerController>();
        ammoBar.maxValue = playerController.weapon.MaxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        ammoBar.value = playerController.weapon.Ammo;
    }
}
