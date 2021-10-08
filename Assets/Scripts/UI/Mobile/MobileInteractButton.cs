using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInteractButton : MobileButtonBase
{
    public override void Apply(Vector2 pos, Camera cam)
    {
        CustomInput.interact = true;
    }

    public override void Reset ()
    {
        CustomInput.interact = false;
    }

}