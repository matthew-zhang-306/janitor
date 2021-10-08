using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileDashButton : MobileButtonBase
{
    public override void Apply(Vector2 pos, Camera cam)
    {
        CustomInput.dash = true;
    }

    public override void Reset ()
    {
        CustomInput.dash = false;
    }

}