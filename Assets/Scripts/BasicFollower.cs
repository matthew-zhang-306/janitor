using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicFollower : MonoBehaviour
{
    public GameObject target;

    private void Start() {
        GetComponent<NavMeshAgent>().updateRotation = false;
        GetComponent<NavMeshAgent>().updateUpAxis = false;
    }

    private void Update() {
        GetComponent<NavMeshAgent>().SetDestination(target.transform.position);
    }
}
