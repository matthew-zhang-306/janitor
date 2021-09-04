using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroomSweepCircle : MonoBehaviour
{
    public float lifetime = 0.2f;

    private void Update() {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
            Destroy(gameObject);    
    }
}
