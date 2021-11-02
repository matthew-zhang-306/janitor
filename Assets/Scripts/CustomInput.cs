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

    public static bool primary;
    public static bool secondary;

    public static InputAction reset;
    public static InputAction close;

    #if DEVELOPMENT_BUILD || UNITY_EDITOR

    public static InputAction DEBUG_roomClear;

    #endif


    public static float GetAxis (string axis)
    {
        // #if (UNITY_ANDROID || UNITY_IPHONE)
        switch (axis) 
        {
            case "Horizontal":
                return moveAxisHx;
            case "Vertical":
                return moveAxisVy;
            default:
                Debug.Log ("throwing axis: " + axis);
                break;
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
        //Used for INGAME button usages. Should not be used for UI.
        //Use callbacks for infrequent requests.
        switch (button) 
        {
            case "Jump":
                return dash;
            case "Interact":
                return interact;
            case "Fire1":
                return primary;
            case "Fire2":
                return secondary;
            default:
                Debug.Log("throwing: " + button);
                break;
        }

        
        return false;
    }
}