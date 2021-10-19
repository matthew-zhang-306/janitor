using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public delegate void CloseEvent ();
    public CloseEvent onClose;

    private ShopItemLootTable[] table;
    public Inventory playerInventory;
    [SerializeField] private Text currency;
    [SerializeField] private TextMeshProUGUI focusText;

    [SerializeField] private GameObject itemParent;
    [SerializeField] private GameObject itemLocations;
    [SerializeField] private GameObject confirmMenu;
    public GameObject buttonPrefab;


    private Dictionary<string, Sprite> sprites;
    private CanvasGroup render;
    
    private bool isFocused = false;
    private string focusedItem = "";

    Tween openTween;
    Tween closeTween;
    public float fadeDuration = 0.5f;

    void Awake()
    {
        render = this.GetComponent<CanvasGroup>();
        
        table = this.GetComponents<ShopItemLootTable>();
    }

    void Start ()
    {
        render = this.GetComponent<CanvasGroup>();
        
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

        int index = 0;
        //Get Sprite List
        foreach (var key in sprites)
        {
            if (index >= itemLocations.transform.childCount) break;

            GameObject button = Instantiate (buttonPrefab, itemParent.transform);
            var sib = button.GetComponent<ShopItemButton>();
            sib.name = key.Key;
            sib.im = key.Value;
            sib.onClickEvent += Focus;

            var rt = button.GetComponent<RectTransform>();
            rt.position = itemLocations.transform.GetChild(index).position;
            // rt.anchoredPosition -= new Vector2Int (index * 100, 50);
            index ++;
            
        }
        confirmMenu.SetActive(false);
              
    }
            
    void OnEnable ()
    {
        render.alpha = 0;
        openTween?.Kill();
        openTween = DOTween.To(() => render.alpha, x => render.alpha = (float) x, 1f, fadeDuration).SetUpdate(true);
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            closeTween?.Kill();
            closeTween = DOTween.To(() => render.alpha, x => render.alpha = (float) x, 0f, fadeDuration).SetUpdate(true);
            Time.timeScale = 1;
            this.Invoke(() => {
                onClose?.Invoke();
                onClose = null;
                PauseMenu.IgnoreEsc = false;
                
            }, fadeDuration + (float) 1e-3);

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
        focusText.text = String.Format ("Purchase {0} for {1}?", focusedItem, InteractableSpawner.i.itemTypes.GetPrice(focusedItem));
    }

    public void Confirm ()
    {
        
        if (focusedItem != "" && isFocused) {
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