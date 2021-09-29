using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour
{
    public string[] targetTags = default;

    public class HitboxEvent : UnityEvent<Collider2D> {}

    // enter and exit events, initialized lazily to guarantee that they will always exist when needed
    private HitboxEvent onTriggerEnter;
    public HitboxEvent OnTriggerEnter => onTriggerEnter ?? (onTriggerEnter = new HitboxEvent());
    private HitboxEvent onTriggerExit;
    public HitboxEvent OnTriggerExit => onTriggerExit ?? (onTriggerExit = new HitboxEvent());

    protected Collider2D coll;

    protected List<Collider2D> otherColliders;
    public bool IsColliding { get {
        CheckOtherColliders();
        return otherColliders != null && otherColliders.Count > 0;
    }}
    
    // The most recent object that entered the collision
    public Collider2D OtherCollider { get {
        return IsColliding ? otherColliders[otherColliders.Count - 1] : null;
    }}
    // All of the objects currently inside the collision
    public List<Collider2D> AllOtherColliders { get {
        CheckOtherColliders();
        return otherColliders;
    }}


    protected virtual void OnEnable() {
        coll = GetComponent<Collider2D>();
        otherColliders = new List<Collider2D>();
    }
    protected virtual void OnDisable() {
        // If the object itself was disabled, clear the entire list
        otherColliders.Clear();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if (targetTags.Any(tag => other.CompareTag(tag))) {
            otherColliders.Add(other);
            OnTriggerEnter?.Invoke(other);
        }
    }
    protected void OnTriggerExit2D(Collider2D other) {
        if (targetTags.Any(tag => other.CompareTag(tag))) {
            otherColliders.Remove(other);
            OnTriggerExit?.Invoke(other);
        }
    }

    void CheckOtherColliders() {
        if (otherColliders == null) return;
        
        // If the object's own collider was disabled, clear the entire list
        if (!coll.enabled || !coll.gameObject.activeInHierarchy) {
            otherColliders.Clear();
            return;
        }

        // Remove any null or disabled colliders from the colliding list
        for (int c = otherColliders.Count - 1; c >= 0; c--) {
            if (otherColliders[c] == null || !otherColliders[c].enabled || !otherColliders[c].gameObject.activeInHierarchy)
                otherColliders.RemoveAt(c);
        }
    }

    // Returns the first collider in the list that passes a certain check function
    public Collider2D GetOtherCollider(Func<Collider2D, bool> check) {
        CheckOtherColliders();
        for (int c = otherColliders.Count - 1; c >= 0; c--) {
            if (check.Invoke(otherColliders[c]))
                return otherColliders[c];
        }

        return null;
    }
}
