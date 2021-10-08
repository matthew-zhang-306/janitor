using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float range = 1f;

    private Camera cam;

    private void Start() {
        // there's a better way to connect this once we have a level manager
        cam = Camera.main;
    }

    private void FixedUpdate() {

        Vector2 mouseViewportPoint = cam.ScreenToViewportPoint(CustomInput.GetMousePosition());

        mouseViewportPoint = new Vector2(Mathf.Clamp01(mouseViewportPoint.x), Mathf.Clamp01(mouseViewportPoint.y));
        transform.position = transform.parent.position + (2 * mouseViewportPoint - Vector2.one).ToVector3() * range;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(range, range, 1f));
    }
}
