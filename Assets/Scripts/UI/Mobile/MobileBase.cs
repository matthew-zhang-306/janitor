using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (RectTransform))]
public class MobileBase : MonoBehaviour
{
    public virtual bool Within (Vector2 pos, Camera cam)
    {
        return false;
    }

    public virtual void Reset ()
    {

    }
    public virtual void Apply (Vector2 pos, Camera cam)
    {

    }
}