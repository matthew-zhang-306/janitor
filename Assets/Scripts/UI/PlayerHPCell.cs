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
    private Color initial;
    private Color final = new Color(.217f, 0.1013f, 0.1013f);
    public void Setup (Health hp, int count, int amt, int maxamt) {
        health = hp;
        this.index = count * maxamt;
        this.amt = amt;
        initial = fill.color;
    }
    public void UpdateFill(float val) {
        
        fill.color = Color.Lerp(initial, final, 1 - val);;
        
    }

    void Update() 
    {
        if (health) {
            UpdateFill (Mathf.Clamp01( (health.GetHealth() - index) / amt));
        }       
    }
}