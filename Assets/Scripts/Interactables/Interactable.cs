using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent (typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    public bool autoInteract = false;

    [SerializeField] protected string _tooltip;

    public virtual string ToolTip
    {
        get => String.Format("[{0}] {1}", PlayerInputMap.sInputMap.FindAction("Interact").GetBindingDisplayString(0, 
            InputBinding.DisplayStringOptions.DontIncludeInteractions
            ) ?? "None", _tooltip);
        private set => _tooltip = value;
    }

    public virtual void OnEnter(PlayerController pc, Inventory i) {}
    public virtual void OnExit(PlayerController pc, Inventory i) {}


    public virtual void DoAction (PlayerController pc, Inventory i)
    {
        Destroy (gameObject);
    }
}