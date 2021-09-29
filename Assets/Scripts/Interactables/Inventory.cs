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

    private int _money;
    public int money {get => _money; set => Mathf.Clamp(_money + value, 0, 1000);}

    private float interactBuffer = 0.2f;
    private bool canInteract = true;

    private List<Interactable> recent;

    void Start () 
    {
        pc = this.GetComponent<PlayerController>();
        recent = new List<Interactable>();
    }

    void Update ()
    {
        //priority to whatever came into context last
        var item = recent.LastOrDefault();
        if (item != null) {
            tooltip.SetActive (true);
            var rt = tooltip.GetComponent<RectTransform>();
            var text = tooltip.GetComponent<Text>();
            rt.position = item.transform.position + tooltipOffset; //RectTransformUtility.WorldToScreenPoint(camera, );
            text.text = item.ToolTip;
        }
        else {
            tooltip.SetActive (false);
        }

        if (Input.GetButton("Interact") && canInteract)
        {
            recent.LastOrDefault()?.DoAction(pc, this);
            canInteract = false;

            //Look at helper.cs
            this.Invoke (()=> canInteract = true, interactBuffer);
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        var item = other.GetComponent<Interactable>();
        if (item != null) {
            if (item.autoInteract) {
                //Should be for cash or such
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