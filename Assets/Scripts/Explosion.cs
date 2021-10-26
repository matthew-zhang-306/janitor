using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float activeTime;
    [SerializeField] private float aliveTime;
    private float timer;
    
    private Collider2D hurtbox;
    [SerializeField] private bool BarrelBomb = false;

    private void Start() {
        timer = 0;
        hurtbox = GetComponent<Collider2D>();
    }

    private void FixedUpdate() {
        timer += Time.deltaTime;

        if (timer > activeTime) {
            hurtbox.enabled = false;
        }

        if (timer > aliveTime) {
            if (BarrelBomb)
            {
                Destroy(transform.parent.gameObject);
            }
            
            Destroy(gameObject);
        }
    }

    
}
