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

    public override bool Within(Vector2 pos, Camera cam)
    {
        bool val = false;
        if (isTouched) {
            val = base.Within(pos, cam);
        }
        else {
            //RIGHT SIDE HARD CODED
            if (pos.x > Screen.width / 2) 
            {
                val = true;
            }
        }

        return val;
    }
}