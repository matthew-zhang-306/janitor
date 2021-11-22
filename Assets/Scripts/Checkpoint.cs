using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public delegate void CheckpointDelegate(Checkpoint checkpoint, Transform playerTransform);
    public CheckpointDelegate OnCheckpointEnter;
    public CheckpointDelegate OnCheckpointExit;


    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, 1f);
        
        Collider2D region = GetComponent<Collider2D>();
        if (region != null) {
            Gizmos.DrawWireCube(region.bounds.center, region.bounds.size);
        }
    }


    // If you're looking for the spot where the player saves their progress
    // when they hit a checkpoint, it's in PlayerController's OnTriggerEnter2D

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            OnCheckpointEnter?.Invoke(this, other.transform);
        }    
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            OnCheckpointExit?.Invoke(this, other.transform);
        }    
    }
}
