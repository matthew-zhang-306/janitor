using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(CanvasGroup))]
public class MobileAxisBase : MobileBase
{
    public GameObject childstick;
    public GameObject reference;
    public RectTransform center;

    protected RectTransform rt;
    protected RectTransform brt;
    protected CanvasGroup render;
    protected bool isTouched = false;

    //used to not spam activation
    protected bool buffer = false;

    private float max_dist;
    private Vector2 initialPos;
    protected virtual void Start ()
    {
        rt = childstick.GetComponent<RectTransform>();
        var cam = this.transform.parent.GetComponent<Canvas>().worldCamera;
        max_dist = Mathf.Abs (Vector2.Distance (cam.WorldToScreenPoint(this.transform.position), 
                cam.WorldToScreenPoint(center.position)));
        brt = GetComponent<RectTransform>();
        render = GetComponent<CanvasGroup>();
        initialPos = rt.anchoredPosition;
    }

    public override void Apply(Vector2 pos, Camera cam)
    {
        if (isTouched) {
            rt.position = cam.ScreenToWorldPoint (pos);
            rt.localPosition = new Vector3 (rt.localPosition.x, rt.localPosition.y, 0);
            rt.anchoredPosition += new Vector2 (-25, 25);        
        }
        else {
            isTouched = true;
            brt.position = cam.ScreenToWorldPoint (pos);
            brt.localPosition = new Vector3 (brt.localPosition.x, brt.localPosition.y, 0);
            brt.anchoredPosition += new Vector2 (-50, 50);
            render.alpha = 1;
        }
    }

    public override bool Within (Vector2 pos, Camera cam)
    {
        var dist = Vector2.Distance (pos, cam.WorldToScreenPoint(center.position));
        return max_dist * 5 >= Mathf.Abs (dist);
    }

    public override void Reset ()
    {
        rt.anchoredPosition = initialPos;
        isTouched = false;
        render.alpha = 0.2f;
    }
}