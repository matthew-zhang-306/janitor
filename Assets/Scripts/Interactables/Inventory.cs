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

    private List<Interactable> recent;

    void Awake () 
    {    
        recent = new List<Interactable>();
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
        
        if (canvas != null) {
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
    }

    void Update ()
    {
        //priority to whatever came into context last
        var item = recent.LastOrDefault();
        if (item != null) {
            tooltip.SetActive (true);
            

            rt.position = item.transform.position + tooltipOffset; //RectTransformUtility.WorldToScreenPoint(camera, );
            text.text = item.ToolTip;
        }
        else {
            tooltip.SetActive (false);
        }

        if (CustomInput.GetButton("Interact") && canInteract)
        {
            recent.LastOrDefault()?.DoAction(pc, this);
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
                Debug.Log ("hi there");
                item.DoAction (pc, this);
            }
            else {
                recent.Add (item);
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

    public void ApplyUpgrade (Upgrade u, object o)
    {
        foreach (Upgradeable comp in upgradeComponents)
        {
            if (o.GetType() == comp.GetType()) {
                comp.ApplyUpgrade (u);
                return;
            }

        }
        Debug.LogError ("Upgrade Fail (component not found / registered)");
    }   
}