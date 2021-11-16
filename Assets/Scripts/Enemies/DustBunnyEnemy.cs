using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustBunnyEnemy : BaseEnemy
{
    private bool shouldPickAction;

    public AudioSource bunnyMove;
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


    private void Update() {
        if (navigator.canNavigate) {
            // set a move animation
            string moveString = "";

            Vector2 moveDir = navigator.GetMoveDirection();
            if (Mathf.Abs(moveDir.x) >= 0.2f) {
                moveString = moveDir.x > 0 ? "MoveRight" : "MoveLeft";
            }
            else if (Mathf.Abs(moveDir.y) >= 0.2f) {
                moveString = moveDir.y > 0 ? "MoveUp" : "MoveDown";
            } else {
                moveString = "Idle";
            }

            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Bunny" + moveString) {
                animator.Play("Bunny" + moveString, 0, 0f);
            }
        }
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

            yield return new WaitForSeconds(0.5f);
        }

        yield return 0;
        shouldPickAction = true;
    }

    protected IEnumerator Action_Shoot() {
        navigator.Stop();
        navigator.ClearPath();

        

        for (int i = 0; i < numShots; i++) {
            
            animator.Play("BunnyAttackNew"); // note: animation is replaced
            yield return new WaitForSeconds(0.58f);

            animator.Play("BunnyIdle");
            var bulletRot = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, player.transform.position - transform.position));
            GameObject.Instantiate(bullet, transform.position, bulletRot).GetComponent<Rigidbody2D>().velocity = bulletRot * Vector2.right * bulletSpeed;
            SoundManager.PlaySound(SoundManager.Sound.BunnyAttack, 1f);

            yield return new WaitForSeconds(1 / fireRate - 0.58f);
        }
        // animator.Play("BunnyAttackNew");
        yield return new WaitForSeconds(rechargeTime);

        shouldPickAction = true;
    }

}
