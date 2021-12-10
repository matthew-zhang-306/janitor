using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// [RequireComponent (typeof(PlayerController))]
public class PlayerInputMap : MonoBehaviour
{
    public static InputActionMap sInputMap;
    public InputActionMap inputMap;

    private InputAction moveAction;
    private InputAction aimAction;

    // private InputAction dashAction;
    // private InputAction interact
    void Awake()
    {
        if (sInputMap == null) {
            sInputMap = inputMap;

            inputMap.Enable();
            // inputMap.FindAction("MoveRight").started += ctx => MoveVertical(ctx, false);
            // inputMap.FindAction("MoveLeft").started += ctx => MoveVertical(ctx, true);
            moveAction = inputMap.FindAction("Move");
            aimAction = inputMap.FindAction("Aim");

            inputMap.FindAction("Jump").started += ctx => Button(ctx, ref CustomInput.dash, true);
            inputMap.FindAction("Jump").canceled += ctx => Button(ctx, ref CustomInput.dash, false);
            inputMap.FindAction("Jump").performed += ctx => { Button(ctx, ref CustomInput.dash, false);

            inputMap.FindAction("Interact").started += ctx => Button(ctx, ref CustomInput.interact, true);
            inputMap.FindAction("Interact").canceled += ctx => Button(ctx, ref CustomInput.interact, false);
            inputMap.FindAction("Interact").performed += ctx => Button(ctx, ref CustomInput.interact, false);

            inputMap.FindAction("Fire1").started += ctx => Button(ctx, ref CustomInput.primary, true);
            inputMap.FindAction("Fire1").canceled += ctx => Button(ctx, ref CustomInput.primary, false);
            inputMap.FindAction("Fire1").performed += ctx => Button(ctx, ref CustomInput.primary, false);

            inputMap.FindAction("Fire2").started += ctx => Button(ctx, ref CustomInput.secondary, true);
            inputMap.FindAction("Fire2").canceled += ctx => Button(ctx, ref CustomInput.secondary, false);
            inputMap.FindAction("Fire2").performed += ctx => Button(ctx, ref CustomInput.secondary, false);

            CustomInput.reset = inputMap.FindAction("Reset");
            CustomInput.close = inputMap.FindAction("Close");

            DontDestroyOnLoad(this);
            DontDestroyOnLoad(gameObject);

            #if DEVELOPMENT_BUILD || UNITY_EDITOR

            CustomInput.DEBUG_roomClear = inputMap.FindAction("Debug_Clear");

            #endif

            
        }
        else {
            inputMap.Disable();
            this.enabled = false;
            Destroy(gameObject);
        }
    }

    void Button (InputAction.CallbackContext ctx, ref bool tf, bool value) 
    {
        tf = value;
    }

    void Update()
    {
        var move = moveAction.ReadValue<Vector2>();
        CustomInput.moveAxisHx = move.x;
        CustomInput.moveAxisVy = move.y;

        #if !(UNITY_ANDROID || UNITY_IPHONE)
        var aim = aimAction.ReadValue<Vector2>();
        CustomInput.axis2x = aim.x;
        CustomInput.axis2y = aim.y;
        #endif
    }

    void MoveVertical (InputAction.CallbackContext ctx, bool reverse)
    {
        CustomInput.moveAxisHx = ctx.ReadValue<float>() * (reverse ? -1 : 1);
    }
}