using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    public bool autoInteract = false;
    public virtual void DoAction (PlayerController pc)
    {

    }
}