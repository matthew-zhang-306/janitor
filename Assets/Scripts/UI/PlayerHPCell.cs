using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (RectTransform))]
public class PlayerHPCell : MonoBehaviour {

    public Image fill;
    
    private Health health;

    private float index;
    private float amt;
    public void Setup (Health hp, int count, int amt) {
        health = hp;
        this.index = count * amt;
        this.amt = amt;

    }
    public void UpdateFill(float val) {
        var temp = fill.color;
        temp.a = val;

        fill.color = temp;

    }

    void Update() 
    {
        if (health) {
            UpdateFill (Mathf.Clamp01( (health.GetHealth() - index) / amt));
        }       
    }
}