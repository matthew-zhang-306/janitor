
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

    public InputAction bindingAction;
    public void OnClick()
    {
        onClickEvent?.Invoke(this.name);
    }

    public void ReBind()
    {
        var rebind = new InputActionRebindingExtensions.RebindingOperation()
            .WithAction(bindingAction)
            .WithCancelingThrough("<Keyboard>/escape");

        rebind.Start();
        rebind.OnComplete(ctx => {Debug.Log("Rebind Complete");SetString();});
    }

    public void SetString()
    {
        keybind.text = String.Format("{0} : {1}", bindingAction.name, bindingAction.bindings[0].effectivePath);
    }
}
