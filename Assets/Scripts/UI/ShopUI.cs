using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public delegate void CloseEvent ();
    public CloseEvent onClose;

    private ShopItemLootTable[] table;
    public Inventory playerInventory;
    [SerializeField] private Text currency;
    [SerializeField] private GameObject itemParent;

    void Start ()
    {
        table = this.GetComponents<ShopItemLootTable>();
        
    }
            
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            onClose?.Invoke();
            onClose = null;
            PauseMenu.IgnoreEsc = false;
        }
    }
}