using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileSweepButton : MobileButtonBase
{
    void Update ()
    {
        var temp = im.color;
        temp.a = CustomInput.melee ? 1f : 0.5f;
        im.color = temp;
    }

    public override void Apply(Vector2 pos, Camera cam)
    {
        CustomInput.melee = true;
        CustomInput.ranged = false;
    }
}