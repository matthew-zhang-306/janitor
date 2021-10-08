using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileButtonBase : MobileBase
{
    protected Image im;
    protected RectTransform rtt;
    public RectTransform center;

    private float max_dist;
    void Start ()
    {
        im = this.GetComponent<Image>();
        rtt = this.GetComponent<RectTransform>();
        var cam = this.transform.parent.GetComponent<Canvas>().worldCamera;
        center.anchoredPosition += new Vector2 (-25, 0);
        max_dist = Mathf.Abs (Vector2.Distance (cam.WorldToScreenPoint(this.transform.position), 
                cam.WorldToScreenPoint(center.position)));
        center.anchoredPosition += new Vector2 (25, 0);
        Debug.Log (max_dist);
    }

    public override bool Within (Vector2 pos, Camera cam)
    {
        var dist = Vector2.Distance (pos, cam.WorldToScreenPoint(center.position));

        return max_dist * 2 >= Mathf.Abs (dist);
    }

}