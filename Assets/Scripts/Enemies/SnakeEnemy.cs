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
        spriteRenderer.color = new Color(1, 0.5f, 1);
        yield return new WaitForSeconds(aimTime);

        // step 2: shoot
        var sequence = DOTween.Sequence();
        for (int i = 0; i < burstLength; i++) {
            sequence.InsertCallback(shootTime * i, SpawnBullet);
        }
        sequence.SetLink(gameObject).SetTarget(this);
        yield return new WaitForSeconds(shootTime * burstLength);

        // step 3: reload
        spriteRenderer.color = Color.white;
        reloadTimer = reloadTime;
    }


    private void SpawnBullet() {
        SnakeBullet projectile =
            Instantiate(bulletPrefab, transform.position, Quaternion.identity)
            .GetComponent<SnakeBullet>();

        // tell the projectile where the player is
        projectile.SetTarget(player.transform.position);
    }

}
