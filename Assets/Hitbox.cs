using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour
{
    [SerializeField]
    protected string[] targetTags = default;

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
        if (targetTags.Any(tag => other.CompareTag(tag)))
            otherColliders.Add(other);
    }
    protected void OnTriggerExit2D(Collider2D other) {
        if (targetTags.Any(tag => other.CompareTag(tag)))
            otherColliders.Remove(other);
    }

    void CheckOtherColliders() {
        if (otherColliders == null) return;
        
        // If the object's own collider was disabled, clear the entire list
        if (!coll.enabled) {
            otherColliders.Clear();
            return;
        }

        // Remove any null or disabled colliders from the colliding list
        for (int c = otherColliders.Count - 1; c >= 0; c--) {
            if (otherColliders[c] == null || otherColliders[c].enabled == false)
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
