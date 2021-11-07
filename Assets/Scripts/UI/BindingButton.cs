
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
        if (isBinding) return;
        isBinding = true;

        var rebind = new InputActionRebindingExtensions.RebindingOperation()
            .WithAction(bindingAction)
            .WithCancelingThrough("<Keyboard>/escape");

        rebind.Start();

        keybind.text = String.Format("{0} : {1}", bindingAction.name, "PRESS ANY");
        rebind.OnComplete(ctx => {
                Debug.Log("Rebind Complete");
                SetString();
                this.Invoke(()=>isBinding = false, 0.5f);
                ctx.Dispose();
            });
    }

    public void SetString()
    {
        keybind.text = String.Format("{0} : {1}", bindingAction.name, bindingAction.bindings[0].effectivePath ?? "None");
    }

    public void Clear()
    {
        bindingAction.ChangeBinding(0).Erase();
    }
}
