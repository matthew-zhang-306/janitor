using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerCameraPos : MonoBehaviour
{
    public float moveDelay;
    public Vector3 moveDir;
    public float moveSpeed;

    private float timer;

    private void FixedUpdate() {
        timer += Time.deltaTime;

        if (timer > moveDelay) {
            transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
        }
    }
}
