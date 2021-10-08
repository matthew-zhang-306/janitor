using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileShootStick : MobileAxisBase
{
    void Update ()
    {
        Vector2 dir = rt.position - reference.transform.position; 
        CustomInput.axis2x = Mathf.Clamp (dir.x, -1, 1);
        CustomInput.axis2y = Mathf.Clamp (dir.y, -1, 1);
    }

}