using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
[RequireComponent (typeof (PlayerController))]
public class Inventory : MonoBehaviour
{
    private PlayerController pc;

    //For upgradables filled out in runtime. (might switch it to inspector stuff)
    private Upgradeable[] upgradeComponents;

    public Vector3 tooltipOffset;
    public GameObject tooltip;
    private RectTransform rt;
    private Text text;


    public Canvas canvas;

    private int _money;
    public int money {get => _money; set => _money = Mathf.Clamp(value, 0, 1000);}

    private int _numKeys;
    public int numKeys { get => _numKeys; set => _numKeys = Mathf.Clamp(value, 0, 4); }

    private float interactBuffer = 0.5f;
    private bool canInteract = true;

    private LinkedList<Interactable> recent;

    protected List<(Upgrade[], System.Type)> upgradeList;

    private GameObject upgradesHolder;

    void Awake () 
    {    
        upgradeList = new List<(Upgrade[], System.Type)>();
        recent = new LinkedList<Interactable>();
        pc = this.GetComponent<PlayerController>();
        upgradeComponents = new Upgradeable[4];
        upgradeComponents[0] = pc;
        upgradeComponents[1] = pc.health;
        upgradeComponents[2] = pc.weapon.weapon;
        upgradeComponents[3] = pc.weapon; 
        // upgradeComponents[3] = pc.weapon.weapon.prefabBullet.GetComponent<BaseProjectile>();
    }

    void Start () 
    {
        
        if (canvas == null) {
            //create canvas here
            Debug.LogWarning ("Inserting Inventory tooltip to first canvas");
            var c = GameObject.Find("Canvas");
            if (c != null) {
                canvas = c.GetComponent<Canvas>();

            }
            else {
                Debug.LogError ("Canvas not found");
            }
        }

        if (!tooltip.scene.IsValid()) {
            //create tooltip from prefab here
            
            tooltip = Instantiate (tooltip, canvas?.transform);
            tooltip.SetActive (false);
        }

        rt = tooltip.GetComponent<RectTransform>();
        text = tooltip.GetComponent<Text>();
        // Destroy(upgradesHolder);
        upgradesHolder = new GameObject("uholder(generated)");
        upgradesHolder.transform.SetParent(this.transform);
    }

    void Update ()
    {
        //priority to whatever came into context last
        var item = recent.LastOrDefault();
        if (item != null) {
            tooltip.SetActive (true);
            

            rt.position = item.transform.position + tooltipOffset; //RectTransformUtility.WorldToScreenPoint(camera, );
            rt.anchoredPosition3D -= Vector3.forward * rt.anchoredPosition3D.z;
            text.text = item.ToolTip;
        }
        else {
            tooltip.SetActive (false);
        }

        if (CustomInput.GetButton("Interact") && canInteract) {
            var interactable = recent.LastOrDefault();

            
            if (interactable) {   
                recent.RemoveLast();
                recent.AddFirst(interactable);
            }

            interactable?.DoAction(pc, this);
            canInteract = false;
            //Look at helper.cs
            this.Invoke (() => canInteract = true, interactBuffer);
            
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        var item = other.GetComponent<Interactable>();
        if (item != null) {
            item.OnEnter(pc, this);

            if (item.autoInteract) {
                //Should be for cash or such
                item.DoAction (pc, this);
            }
            else {
                recent.AddLast (item);
            }
        }
    }

    void OnTriggerExit2D (Collider2D other)
    {
        var item = other.GetComponent<Interactable>();
        if (item != null) {
         //   if (CustomInput.GetButton("Interact")) { SoundManager.PlaySound(SoundManager.Sound.Med, 0.5f); }
            item.OnExit(pc, this);
            recent.Remove (item);
        }
    }

    public void ApplyUpgrade (Upgrade u, System.Type o)
    {
        foreach (Upgradeable comp in upgradeComponents)
        {
            // Debug.Log(o.GetType());
            // Debug.Log(comp.GetType());
            // Debug.Log(comp.GetType().IsInstanceOfType(o));
            
            if (o.IsInstanceOfType(comp)) {
                comp.ApplyUpgrade (u);
                return;
            }

        }
        Debug.LogError ("Upgrade Fail (component not found / registered)");
    }   

    public void ResetAndApplyAllUpgrades (InventorySnapShot iss)
    {
        foreach (Upgradeable comp in upgradeComponents)
        {
            comp.Reset();

        }
        foreach (var u in iss.ulist) {
            ApplyUpgrade(u.Item1, u.Item2);
        }
    }

    public void SaveUpgrade (Upgrade[] sui, System.Type t)
    {
        // sui.gameObject.SetActive(false);
        upgradeList.Add((sui, t));
        // sui.transform.SetParent(this.upgradesHolder.transform);
    }

    public class InventorySnapShot
    {
        int moneyss;
        int keyss;

        public List <(Upgrade, System.Type)> ulist;


        public InventorySnapShot (Inventory inv)
        {
            moneyss = inv._money;
            keyss = inv._numKeys;
            ulist = new List<(Upgrade, System.Type)>();
            foreach (var sui in  inv.upgradeList) {
                foreach (var upgrade in sui.Item1) {
                    ulist.Add((upgrade, sui.Item2));
                }
            }
        }

        public void Apply (Inventory inv)
        {
            inv.ResetAndApplyAllUpgrades(this);

        }

    }
}