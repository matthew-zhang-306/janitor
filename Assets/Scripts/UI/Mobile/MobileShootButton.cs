using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileShootButton : MobileButtonBase
{

    void Update ()
    {
        var temp = im.color;
        temp.a = CustomInput.ranged ? 1f : 0.5f;
        im.color = temp;
    }

    public override void Apply(Vector2 pos, Camera cam)
    {
        CustomInput.ranged = true;
        CustomInput.melee = false;
    }

}