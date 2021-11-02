using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaperEnemy : BaseEnemy
{
    [SerializeField] private float wanderSpeed = default;
    [SerializeField] private float wanderTargetDistance = default;
    [SerializeField] private float chargeSpeed = default;
    [SerializeField] private float chargeAcceleration = default;
    [SerializeField] private float chargeTurnSpeed = default;
    [SerializeField] private float chargeStartTime = default;

    [SerializeField] private Hitbox explosionTrigger = default;
    [SerializeField] private GameObject explosionPrefab = default;

    private bool shouldSelectAction = true;
    private float canSeePlayerTimer;
    private float repathTimer;

    protected override void Start() {
        base.Start();

        navigator.speed = wanderSpeed;
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if (!CanAct) {
            canSeePlayerTimer = 0;
            repathTimer = 0;
            return;
        }

        Vector2 raycastDir = player.transform.position - transform.position;
        if (Physics2D.Raycast(transform.position, raycastDir, raycastDir.magnitude, LayerMask.GetMask("Wall"))) {
            canSeePlayerTimer = 0;
        }
        else {
            canSeePlayerTimer += Time.deltaTime;
        }

        repathTimer = Mathf.MoveTowards(repathTimer, 0, Time.deltaTime);
        
        if (shouldSelectAction) {
            // select action
            string selectedAction = "";
            if (canSeePlayerTimer >= 0.1f) {
                selectedAction = "Charge";
            }
            else {
                selectedAction = "Wander";
            }

            //If action was specified in inspector then go ahead and do the action
            if (actionTable.ContainsKey(selectedAction)) {
                StartCoroutine(actionTable[selectedAction]());
            }
        }
    }


    private IEnumerator Action_Wander() {
        if (repathTimer > 0)
            // wait to refresh path
            yield break;
        
        Vector3 randomDir = Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector3.right;
        navigator.SetDestination(transform.position + randomDir * wanderTargetDistance);
        repathTimer = 1f;
    }

    private IEnumerator Action_Charge() {
        shouldSelectAction = false;
        navigator.Stop();
        
        // step 1: spotting the player animation
        Vector3 chargeDirection = (player.transform.position - transform.position).normalized;
        for (float startTime = 0f; startTime < chargeStartTime; startTime += Time.deltaTime) {
            // the enemy backs up slightly first
            rb2d.velocity = -chargeDirection * wanderSpeed * Mathf.Lerp(2, 0, startTime / chargeStartTime);
            yield return new WaitForFixedUpdate();
        }

        // step 2: set up the charge
        GetComponent<Collider2D>().enabled = false;
        floorMarker.GetComponent<FloorMarker>().markAmount *= 10;
        rb2d.velocity = Vector2.zero;
        SoundManager.PlaySound(SoundManager.Sound.Diaper, 1f);
        // step 3: charge
        while (!explosionTrigger.IsColliding) {
            chargeDirection = (player.transform.position - transform.position).normalized;
            if (rb2d.velocity.magnitude > 0) {
                // turn toward the charge direction
                float fullRotateAngle = Vector2.SignedAngle(rb2d.velocity, chargeDirection);
                rb2d.velocity = Quaternion.Euler(0, 0, Mathf.Clamp(fullRotateAngle, -chargeTurnSpeed, chargeTurnSpeed)) * rb2d.velocity;

                // acceleration to max speed
                rb2d.velocity = Vector2.MoveTowards(rb2d.velocity, rb2d.velocity.normalized * chargeSpeed, chargeAcceleration);
            } else {
                rb2d.velocity = chargeDirection * chargeAcceleration;
            }

            yield return new WaitForFixedUpdate();
        }

        // step 4: explode
        Explode();
    }


    private void Explode() {
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        onDeath?.Invoke(this);
        onDeath = null;

        Die();
    }
}
