using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileMoveStick : MobileAxisBase
{
    
    
    void Update ()
    {
        Vector2 dir = rt.position - reference.transform.position; 
        CustomInput.moveAxisHx = Mathf.Clamp (dir.x, -1, 1);
        CustomInput.moveAxisVy = Mathf.Clamp (dir.y, -1, 1);

    }
    public override bool Within(Vector2 pos, Camera cam)
    {
        bool val = false;
        if (isTouched) {
            val = base.Within(pos, cam);
        }
        else {
            //LEFT SIDE HARD CODED
            if (pos.x <= Screen.width / 2) 
            {
                val = true;
            }
        }

        return val;
    }

}