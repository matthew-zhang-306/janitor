using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
[RequireComponent (typeof (PlayerController))]
public class Inventory : MonoBehaviour
{
    private PlayerController pc;
    public Vector3 tooltipOffset;
    public GameObject tooltip;
    private RectTransform rt;
    private Text text;


    public Canvas canvas;

    private int _money;
    public int money {get => _money; set => Mathf.Clamp(_money + value, 0, 1000);}

    private float interactBuffer = 0.2f;
    private bool canInteract = true;

    private List<Interactable> recent;

    void Awake () {
        
        recent = new List<Interactable>();
    }

    void Start () 
    {
        pc = this.GetComponent<PlayerController>();
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
            recent.Remove (item);
        }
    }
}