using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInteract : Interactable
{
    private bool opened = false;
    
    
    public GameObject ShopUICanvas;


    // Start is called before the first frame update
    void Start()
    {
        if (!ShopUICanvas.scene.IsValid()) {
            //create tooltip from prefab here
            
            ShopUICanvas = Instantiate (ShopUICanvas);
            ShopUICanvas.SetActive (opened);
        }

    }
    public override string ToolTip
    {
        get => _tooltip + "(Access Kiosk)";
    }
    public override void DoAction (PlayerController pc, Inventory _)
    {
        //open shop UI
        opened = !opened;
        ShopUICanvas.SetActive (opened);
        if (opened)
        {
            var shop = ShopUICanvas.GetComponent<ShopUI>();
            shop.playerInventory = pc.inventory;
            shop.onClose += () => {opened = false; ShopUICanvas.SetActive(false);};
            PauseMenu.IgnoreEsc = true;
            Time.timeScale = 0;
        }
        
    }
    
}
