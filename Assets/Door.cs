using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator animator;
    public Collider2D[] colliders;
    bool isOpened;

    private void Awake() {
        animator = GetComponent<Animator>();
        isOpened = true;
        DisableCollision();
    }

    public void OpenDoor() {
        if (isOpened)
            return;
        isOpened = true;
        animator.SetTrigger("Open");
    }
    public void CloseDoor() {
        if (!isOpened)
            return;
        isOpened = false;
        animator.SetTrigger("Closed");
    }


    // called by the animator
    public void EnableCollision() {
        foreach (var collider in colliders) {
            collider.enabled = true;
        }
    }
    public void DisableCollision() {
        foreach (var collider in colliders) {
            collider.enabled = false;
        }
    }
}
