using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileMoveStick : MobileAxisBase
{

    void Update ()
    {
        Vector2 dir = rt.position - reference.transform.position; 
        CustomInput.axis1x = Mathf.Clamp (dir.x, -1, 1);
        CustomInput.axis1y = Mathf.Clamp (dir.y, -1, 1);
    }

}