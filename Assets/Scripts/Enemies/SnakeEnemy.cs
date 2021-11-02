using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SnakeEnemy : BaseEnemy
{

    [SerializeField] private float startTime;
    [SerializeField] private float reloadTime;
    [SerializeField] private float aimTime;
    [SerializeField] private float shootTime;
    [SerializeField] private int burstLength;
    private float reloadTimer;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnLocation;
    private bool oldCanAct;

    protected override void Start() {
        base.Start();
        reloadTimer = startTime;
        oldCanAct = CanAct;
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
    
        if (!CanAct) {
            if (oldCanAct) {
                StopAllCoroutines();
            }
            reloadTimer = startTime;
            return;
        }

        reloadTimer = Mathf.MoveTowards(reloadTimer, 0f, Time.deltaTime);

        if (reloadTimer == 0f) {
            StartCoroutine(Action_Shoot());
        }

        oldCanAct = CanAct;
    }


    private IEnumerator Action_Shoot() {
        // make sure the snake doesn't shoot while still shooting by setting reloadTimer to something very massive
        reloadTimer = 9999;

        // step 1: aim (up)
        animator.Play("SnakeCharge");
        yield return new WaitForSeconds(aimTime);

        // step 2: shoot
        for (int i = 0; i < burstLength; i++) {
            float attackAnimDelay = 0.15f;
            float chargeAnimDelay = 0.17f;

            animator.Play("SnakeAttack");
            yield return new WaitForSeconds(attackAnimDelay);
            SpawnBullet();
            yield return new WaitForSeconds(shootTime - chargeAnimDelay);
            if (i != burstLength - 1) {
                animator.Play("SnakeCharge");
            }
            yield return new WaitForSeconds(chargeAnimDelay);
        }

        // step 3: reload
        animator.Play("SnakeIdle");
        reloadTimer = reloadTime;
    }


    private void SpawnBullet() {
        SnakeBullet projectile =
            Instantiate(bulletPrefab, bulletSpawnLocation.position, Quaternion.identity)
            .GetComponent<SnakeBullet>();
        SoundManager.PlaySound(SoundManager.Sound.Snake, 1f);
        // tell the projectile where the player is
        projectile.SetTarget(player.transform.position);

    }

}
