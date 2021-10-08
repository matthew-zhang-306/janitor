using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileAxisBase : MobileBase
{
    public GameObject childstick;
    public GameObject reference;
    public RectTransform center;

    protected RectTransform rt;

    private float max_dist;
    void Start ()
    {
        rt = childstick.GetComponent<RectTransform>();
        var cam = this.transform.parent.GetComponent<Canvas>().worldCamera;
        max_dist = Mathf.Abs (Vector2.Distance (cam.WorldToScreenPoint(this.transform.position), 
                cam.WorldToScreenPoint(center.position)));

    }

    public override void Apply(Vector2 pos, Camera cam)
    {
        rt.position = cam.ScreenToWorldPoint (pos);
        rt.localPosition = new Vector3 (rt.localPosition.x, rt.localPosition.y, 0);
        rt.anchoredPosition += new Vector2 (-25, 25);
    }

    public override bool Within (Vector2 pos, Camera cam)
    {
        var dist = Vector2.Distance (pos, cam.WorldToScreenPoint(center.position));
        return max_dist * 1.5 >= Mathf.Abs (dist);
    }

    public override void Reset ()
    {
        rt.anchoredPosition = Vector2.zero;
    }
}