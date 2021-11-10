
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System;

[RequireComponent(typeof (Button))]
public class BindingButton : MonoBehaviour
{
    public delegate void ButtonDelegate (string name);
    public ButtonDelegate onClickEvent;

    public TextMeshProUGUI keybind;

    [HideInInspector]
    public InputAction bindingAction;

    [HideInInspector]
    public int index;

    private bool isBinding;

    public void OnClick()
    {
        onClickEvent?.Invoke(this.name);
    }

    public void ReBind()
    {
        //Buffer so you don't accidentally spam binding operations
        if (isBinding) return;
        isBinding = true;

        //Begin rebinding
        // if (bindingAction)
        // var rebind = new InputActionRebindingExtensions.RebindingOperation()
        //     .WithAction(bindingAction)
        //     .WithCancelingThrough("<Keyboard>/escape")
        //     .WithTargetBinding(index);

        var rebind = bindingAction.PerformInteractiveRebinding()
            .WithCancelingThrough("<Keyboard>/escape")
            .WithControlsExcluding("Mouse/delta")
            .WithControlsExcluding("Mouse/position")
            .WithExpectedControlType("Button")
            .WithTargetBinding(index);
        rebind.Start();

        //For composite bindings (move for now)
        
        keybind.text = String.Format("{0} : {1}", bindingAction.name, "PRESS ANY");
        
        
        rebind.OnComplete(ctx => {
            Debug.Log("Rebind Complete");
            SetString();
            this.Invoke(()=>isBinding = false, 0.5f);
            ctx.Dispose();
        });
        rebind.OnCancel(ctx => {
            Debug.Log("Rebind Canceled");
            SetString();
            isBinding = false;
            ctx.Dispose();
        });
    }

    public void SetString()
    {
        keybind.text = String.Format("{0} : {1}", bindingAction.name, 
            bindingAction.GetBindingDisplayString(index, 
            //InputBinding.DisplayStringOptions.DontOmitDevice | 
            InputBinding.DisplayStringOptions.DontIncludeInteractions
            ) ?? "None");
    }

    public void Clear()
    {
        bindingAction.ChangeBinding(index).Erase();
    }
}
