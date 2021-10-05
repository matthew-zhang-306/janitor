using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        
        Collider2D region = GetComponent<Collider2D>();
        if (region != null) {
            Gizmos.DrawWireCube(region.bounds.center, region.bounds.size);
        }
    }


    // If you're looking for the spot where the player saves their progress
    // when they hit a checkpoint, it's in PlayerController's OnTriggerEnter2D
}
