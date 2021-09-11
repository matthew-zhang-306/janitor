using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatEnemy : BaseEnemy
{
    public float seekSpeed;
    public float startupTime;
    private float startupTimer;

    private string[] actions; // daniel, refactor to use this when you need to

    protected override void Start() {
        base.Start();
        navMeshAgent.speed = seekSpeed;
        startupTimer = Random.Range(0, startupTime);
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if (CanAct) {
            startupTimer = Mathf.MoveTowards(startupTimer, 0, Time.deltaTime);
            if (startupTimer == 0) {
                // pick something to do this frame
                Action_Seek();
            }
            else {
                navMeshAgent.isStopped = true;
            }

            navMeshAgent.speed = invincibilityTimer == 0 ? seekSpeed : seekSpeed / 3f;
        }
        else {
            if (startupTimer == 0)
                startupTimer = Random.Range(0, startupTime);
        }
    }

    protected override void TakeDamage(Collider2D other) {
        base.TakeDamage(other);

        float per = (1 - health.GetHealthPercent())/2 + health.GetHealthPercent();
    }


    private void Action_Seek() {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(player.transform.position);
    }
}
