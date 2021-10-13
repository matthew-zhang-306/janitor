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
    [SerializeField] private GameObject confirmMenu;
    public GameObject buttonPrefab;

    private Dictionary<string, Sprite> sprites;
    
    private bool isFocused = false;
    private string focusedItem = "";

    void Start ()
    {
        table = this.GetComponents<ShopItemLootTable>();
        sprites = new Dictionary<string, Sprite>();

        foreach (var lt in table) {
            string name = lt.GetItem();
            var go = InteractableSpawner.i.GetItem(name);
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr == null) {
                sr = go.GetComponentInChildren<SpriteRenderer>();
                if (sr == null) {
                    Debug.LogWarning("Sprite not found from interactable");
                    continue;
                }
            }
            sprites[name] = sr.sprite;
        }

        int index = -sprites.Count / 2;
        //Get Sprite List
        foreach (var key in sprites)
        {
            GameObject button = Instantiate (buttonPrefab, itemParent.transform);
            var sib = button.GetComponent<ShopItemButton>();
            sib.name = key.Key;
            sib.im = key.Value;
            sib.onClickEvent += Focus;

            var rt = button.GetComponent<RectTransform>();
            rt.anchoredPosition -= new Vector2Int (index * 100, 50);
            index++;
        }
        confirmMenu.SetActive(false);
    }
            
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            onClose?.Invoke();
            onClose = null;
            PauseMenu.IgnoreEsc = false;
            Time.timeScale = 1;
        }
    }

    void Spawn (string name)
    {
        InteractableSpawner.i.SpawnItem(name, playerInventory.transform.position);
    }

    void Focus (string name)
    {
        //bring up confirmation window 
        //also include detail?
        isFocused = true;
        confirmMenu.SetActive(true);
        focusedItem = name;
    }

    public void Confirm ()
    {
        
        if (focusedItem != "") {
            var price = InteractableSpawner.i.itemTypes.GetPrice(focusedItem);
            if (playerInventory.money - price >= 0) {
                Debug.Log("confirming");
                playerInventory.money -= price;
                Spawn (focusedItem);

                DeFocus();
            }
            else {
                Debug.Log ("Buy failed");
            }
        }
    }
    public void DeFocus ()
    {
        isFocused = false;
        confirmMenu.SetActive(false);
        focusedItem = "";
    }
}