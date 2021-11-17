using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using System;


public class AmmoUI : MonoBehaviour
{

    public GameObject Player;
    public Slider ammoBar;
    private PlayerController playerController;
    public TextMeshProUGUI text;
    private static NumberFormatInfo nfi = new CultureInfo( "en-US", false ).NumberFormat;


    // Start is called before the first frame update
    void Start()
    {
        playerController = Player.GetComponent<PlayerController>();
        ammoBar.maxValue = playerController.weapon.MaxAmmo;
        nfi.NumberDecimalDigits = 0;
    }

    // Update is called once per frame
    void Update()
    {
        ammoBar.value = playerController.weapon.Ammo;
        ammoBar.maxValue = playerController.weapon.MaxAmmo;
        float drain = playerController.weapon?.weapon?.AmmoDrain ?? 0;
        if (drain != 0)
            text.text = String.Format (nfi, "{0:N} / {1:N}", ammoBar.value / drain, ammoBar.maxValue / drain);
    }
}
