using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public static class CustomInput
{
    public static float moveAxisHx;
    public static float moveAxisVy;
    public static float axis2x;
    public static float axis2y;

    public static bool melee;
    public static bool ranged;

    public static bool dash;
    public static bool interact;
    public static float GetAxis (string axis)
    {
        // #if (UNITY_ANDROID || UNITY_IPHONE)
        switch (axis) 
        {
            case "Horizontal":
                return moveAxisHx;
            case "Vertical":
                return moveAxisVy;
        }

        // #else
        // return Input.GetAxisRaw (axis);
        // #endif
        
        return 0f;
    }

    public static Vector2 GetMousePosition ()
    {
        #if (UNITY_ANDROID || UNITY_IPHONE)
        return new Vector2 (Screen.width / 2 + axis2x * 10, Screen.height / 2 + axis2y * 10);
        #else
        return Mouse.current.position.ReadValue();
        #endif
    }
    public static bool GetButton (string button)
    {
        // #if (UNITY_ANDROID || UNITY_IPHONE)
        switch (button) 
        {
            case "Jump":
                return dash;
            case "Interact":
                return interact;
        }

        // #else
        // return Input.GetButton (button);
        // #endif
        
        return false;
    }
}