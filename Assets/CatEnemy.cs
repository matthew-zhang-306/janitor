using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CatEnemy : BaseEnemy
{
    private bool shouldPickAction;

    public float moveSpeed;
    public float maxMoveDistance;
    public float desiredDistanceFromPlayer;
    public float playerLookahead;
    public float chargeTime;
    public float attackTime;
    public float rechargeTime;
    public float dirtyTime;

    private float rechargeTimer;
    private float shouldRefreshPathTimer;

    public GameObject shockwavePrefab;

    protected override void Start() {
        base.Start();

        navigator.speed = moveSpeed;
        rechargeTimer = rechargeTime;
        shouldRefreshPathTimer = 0f;
        shouldPickAction = true;

        // copied from the slime
        DOTween.Sequence()
            .InsertCallback(0f, () => floorMarker.SetActive(true))
            .InsertCallback(0.2f, () => floorMarker.SetActive(false))
            .InsertCallback(dirtyTime, () => floorMarker.SetActive(true))
            .SetLoops(-1, LoopType.Restart)
            .SetLink(gameObject);
    }

    protected override void FixedUpdate() {
        if (!CanAct) {
            rechargeTimer = rechargeTime;
            return;
        }

        rechargeTimer = Mathf.MoveTowards(rechargeTimer, 0f, Time.deltaTime);
        shouldRefreshPathTimer += Time.deltaTime;

        if (shouldPickAction) {
            string selectedAction = "";
            if (rechargeTimer == 0f) {
                selectedAction = "Attack";
            }
            else {
                selectedAction = "Move";
            }

            //If action was specified in inspector then go ahead and do the action
            if (actionTable.ContainsKey(selectedAction)) {
                shouldPickAction = false;
                StartCoroutine(actionTable[selectedAction]());
            }
        }
    }

    private IEnumerator Action_Attack() {
        shouldPickAction = false;
        navigator.canNavigate = false;

        spriteRenderer.color = Color.blue;
        yield return new WaitForSeconds(chargeTime);
        
        spriteRenderer.color = Color.red;
        Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(attackTime);

        spriteRenderer.color = Color.white;
        shouldPickAction = true;
        navigator.canNavigate = true;
        rechargeTimer = rechargeTime;
    }

    private IEnumerator Action_Move() {
        shouldPickAction = false;

        if (shouldRefreshPathTimer >= 1f) {
            shouldRefreshPathTimer -= 1f;

            // decide where to go
            Vector3 playerPoint =
                player.transform.position + player.Velocity.ToVector3() * playerLookahead;
            Vector3 targetPoint =
                playerPoint + (transform.position - playerPoint).normalized * desiredDistanceFromPlayer;
            Vector3 desiredPoint =
                Vector3.MoveTowards(transform.position, targetPoint, maxMoveDistance);
            
            navigator.SetDestination(desiredPoint);
        }
        
        yield return new WaitForSeconds(0.2f);
        shouldPickAction = true;
    }
}
