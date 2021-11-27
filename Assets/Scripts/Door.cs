using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public Collider2D[] colliders;
    bool isOpened;

    private void Awake() {
        isOpened = true;
        DisableCollision();
    }

    public void OpenDoor() {
        if (isOpened)
            return;
        isOpened = true;
        animator.SetTrigger("Open");
        SoundManager.PlaySound(SoundManager.Sound.DoorOpen, 0.5f);
    }
    public void CloseDoor() {
        if (!isOpened)
            return;
        isOpened = false;
        animator.SetTrigger("Closed");
        SoundManager.PlaySound(SoundManager.Sound.DoorClose, 0.5f);
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
