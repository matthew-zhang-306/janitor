using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNavigator : MonoBehaviour
{
    [HideInInspector] public Pathfinding pathfinding;

    public bool canNavigate;
    private Pathfinding.PathRequest currentRequest;
    public bool isRequestingPath => currentRequest != null;
    public bool isFollowingPath { get; private set; }

    public float unitSize;
    private Queue<Vector3> currentPath;

    public float speed;
    public new Rigidbody2D rigidbody { get; private set; }


    private void Start() {
        currentPath = new Queue<Vector3>();
        rigidbody = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate() {
        if (!canNavigate)
            return;

        if (currentRequest != null) {
            // update the start position of the outgoing request until it is handled
            currentRequest.startPos = transform.position;
        }
        
        if (currentPath.Count > 0) {
            Vector3 waypoint = currentPath.Peek();
            if (Vector3.Distance(waypoint, transform.position) <= 0.5f) {
                currentPath.Dequeue();
                waypoint = currentPath.Count > 0 ? currentPath.Peek() : transform.position;
            }

            rigidbody.velocity = (waypoint - transform.position).normalized * speed;
        }
        else {
            rigidbody.velocity = Vector2.zero;
        }

        isFollowingPath = currentPath.Count > 0;
    }


    public void SetDestination(Vector3 dest, System.Action callback = null) {
        canNavigate = true;
        System.Action<List<Vector3>> fullCallback = path => {
            currentPath = new Queue<Vector3>(path);
            currentRequest = null;

            if (callback != null)
                callback();
        };


        if (currentRequest != null) {
            // update the end position of our previous unfulfilled request
            currentRequest.endPos = dest;
            currentRequest.callback = fullCallback;
        }
        else {
            if (pathfinding == null) {
                Debug.LogError("wtf" + dest.x + " " + dest.y + ", " + transform.position);
            }
            // ask for a new path
            currentRequest = pathfinding.RequestPath(transform.position, dest, unitSize, fullCallback);
        }
    }


    public Vector2 GetMoveDirection() {
        if (currentPath.Count == 0) {
            return rigidbody.velocity;
        }

        return (currentPath.Peek() - transform.position).normalized;
    }


    public void Stop() {
        canNavigate = false;
        rigidbody.velocity = Vector2.zero;
    }

    public void ClearPath() {
        currentPath.Clear();
    }
}
