﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public PlayerController Player;
    private Health health;

    [SerializeField] private int hpPerCell = 10;
    public PlayerHPCell hpCellPrefab;

    public Transform cellContainer;
    private int cellCount;
    // Start is called before the first frame update
    void Start()
    {
        health = Player.GetComponent<Health>();
        SetCells();
    }

    // Update is called once per frame
    void Update()
    {
        // healthBar.maxValue = health.GetMaxHealth();
        // healthBar.value = health.GetHealth();
        if (health.GetMaxHealth() - cellCount * hpPerCell > 0) {
            Debug.Log("health changed");
            SetCells();
        }

    }

    void SetCells () 
    {
        cellCount = 0;

        int mhp = health.GetMaxHealth();

        foreach (Transform child in cellContainer) {
            if (child.gameObject.GetComponent<PlayerHPCell>())
                Destroy(child.gameObject);  
        }

        int mhptotal = mhp;
        for (int i = 0; i < mhp; i += hpPerCell) {
            var go = Instantiate<PlayerHPCell>(hpCellPrefab, cellContainer);
            var rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition += new Vector2(rect.rect.width * cellCount, 0);

            //Make sure last cell scales 
            int amt = Mathf.Min(mhptotal, hpPerCell);
            mhptotal -= amt;

            go.Setup(health, cellCount, amt, hpPerCell);
            cellCount++;
        }

        if (mhptotal != 0) {
            Debug.LogWarning("HP CELLS NOT SCALED");
        }
    }
}
