using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustBunnyEnemy : BaseEnemy
{
    private bool shouldPickAction;

    public float moveSpeed;
    public float fireRate;
    public float rechargeTime;
    public int numShots;

    private float canSeePlayerTimer;
    private float shouldRefreshPathTimer;

    public GameObject bullet;
    public float bulletSpeed;


    protected override void Start() {
        base.Start();

        navigator.speed = moveSpeed;
        shouldPickAction = true;
    }


    protected override void FixedUpdate() {
        base.FixedUpdate();

        Vector2 raycastDir = player.transform.position - transform.position;
        if (Physics2D.Raycast(transform.position, raycastDir, raycastDir.magnitude, LayerMask.GetMask("Wall"))) {
            canSeePlayerTimer = 0;
        }
        else {
            canSeePlayerTimer += Time.deltaTime;
        }

        if (CanAct && shouldPickAction) {
            string selectedAction = "";

            if (canSeePlayerTimer >= 0.5f) {
                selectedAction = "Shoot";
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
        else if (!CanAct) {
            canSeePlayerTimer = 0;
            shouldRefreshPathTimer = 0;
        }

        spriteRenderer.color = invincibilityTimer > 0 ? new Color(1, 0.4f, 0.4f) : Color.white;
    }


    protected IEnumerator Action_Move() {
        shouldRefreshPathTimer -= Time.deltaTime;
        if (shouldRefreshPathTimer <= 0f) {
            shouldRefreshPathTimer += 1f;

            navigator.canNavigate = true;
            navigator.SetDestination(player.transform.position, null);
        }

        yield return 0;
        shouldPickAction = true;
    }

    protected IEnumerator Action_Shoot() {
        navigator.Stop();
        navigator.ClearPath();

        for (int i = 0; i < numShots; i++) {
            var bulletRot = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, player.transform.position - transform.position));
            GameObject.Instantiate(bullet, transform.position, bulletRot).GetComponent<Rigidbody2D>().velocity = bulletRot * Vector2.right * bulletSpeed;

            yield return new WaitForSeconds(1 / fireRate);
        }

        yield return new WaitForSeconds(rechargeTime);

        shouldPickAction = true;
    }

}
