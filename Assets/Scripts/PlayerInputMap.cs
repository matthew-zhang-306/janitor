using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(PlayerController))]
public class PlayerInputMap : MonoBehaviour
{
    public InputActionMap inputMap;

    private InputAction moveAction;

    // private InputAction dashAction;
    // private InputAction interact
    void Start()
    {
        inputMap.Disable();
        // inputMap.FindAction("MoveRight").started += ctx => MoveVertical(ctx, false);
        // inputMap.FindAction("MoveLeft").started += ctx => MoveVertical(ctx, true);
        moveAction = new InputAction();

        moveAction.AddCompositeBinding("2DVector")
            .With("Right", inputMap.FindAction("MoveRight").bindings[0].path)
            .With("Left", inputMap.FindAction("MoveLeft").bindings[0].path)
            .With("Up", inputMap.FindAction("MoveUp").bindings[0].path)
            .With("Down", inputMap.FindAction("MoveDown").bindings[0].path);

        moveAction.Enable();
    }

    void Update()
    {
        var move = moveAction.ReadValue<Vector2>();
        CustomInput.moveAxisHx = move.x;
        CustomInput.moveAxisVy = move.y;
    }

    void MoveVertical (InputAction.CallbackContext ctx, bool reverse)
    {
        CustomInput.moveAxisHx = ctx.ReadValue<float>() * (reverse ? -1 : 1);
    }
}