using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LargeSlimeBoss : BaseEnemy
{
    public GameObject bossHealthBarPrefab;
    public GameObject shockwavePrefab;
    public GameObject dashIndicator;
    private BossHealthBar healthBar;

    [SerializeField] private float seekSpeed = default;
    [SerializeField] private float dashSpeed = default;
    [SerializeField] private float moveTime = default;
    [SerializeField] private float dashAimTime = default;
    [SerializeField] private float dashLockTime = default;
    [SerializeField] private float dirtyTime = default;
    [SerializeField] private int dashIntervalMin = default;
    [SerializeField] private int dashIntervalMax = default;
    [SerializeField] private float shockwaveDelay = default;
    private bool shouldPickAction;
    private int movesUntilDash;
    private bool shouldAimDash;


    protected override void Start()
    {
        base.Start();
        dashIndicator.SetActive(false);
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if (shouldAimDash) {
            dashIndicator.transform.rotation = Quaternion.Euler(0, 0, 
                Vector2.SignedAngle(Vector2.right, player.transform.position - transform.position)
            );
            // animator.Play("SlimeFace" + GetDirectionString(dashIndicator.transform.right));
        }

        if (CanAct && shouldPickAction) {
            string selectedAction = "";

            if (movesUntilDash == 0) {
                selectedAction = "Dash";
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


    public override void OnInit() {
        // copied from slime
        DOTween.Sequence()
            .InsertCallback(0f, () => floorMarker.SetActive(true))
            .InsertCallback(0.2f, () => floorMarker.SetActive(false))
            .InsertCallback(dirtyTime, () => floorMarker.SetActive(true))
            .SetLoops(-1, LoopType.Restart)
            .SetLink(gameObject);
        
        shouldPickAction = true;
        shouldAimDash = false;
        movesUntilDash = 1;

        healthBar = Instantiate(bossHealthBarPrefab).GetComponent<BossHealthBar>();
        healthBar.Init(this, "Red Slime");
    }

    protected override void Die() {
        Helpers.Invoke(healthBar, healthBar.Destroy, 2f);
        base.Die();
    }

    protected override void OnPlayerDied(PlayerController player) {
        base.OnPlayerDied(player);

        StopAllCoroutines();
        shouldAimDash = false;
        shouldPickAction = false;

        Helpers.Invoke(this, () => {
            healthBar?.Destroy();
            healthBar = null;
        }, 1f);
    }


    // same as the slime
    private IEnumerator Action_Move ()
    {
        movesUntilDash--;

        navigator.SetDestination(player.transform.position, () => {
            navigator.DOKill();
            navigator.speed = seekSpeed;
            DOTween.Sequence()
                .Append(DOTween.To(() => navigator.speed, s => navigator.speed = s, seekSpeed / 4f, moveTime))
                .InsertCallback(0.05f, () => {
                    Debug.Log("big slime playing animation " + GetDirectionString(rb2d.velocity));

                    // yes! you have to specify the two other parameters -1 and 0.
                    // for some reason animator.Play(clip) works COMPLETELY DIFFERENTLY from
                    // animator.Play(clip, number, number) if you try to play an animation
                    // that the animator is already playing. it's extremely dumb
                    animator.Play("SlimeMove" + GetDirectionString(rb2d.velocity), -1, 0);
                })
                .SetLink(gameObject).SetTarget(navigator);
        });
        
        yield return new WaitForSeconds(moveTime);
        shouldPickAction = true;
    }

    private IEnumerator Action_Dash ()
    {
        // aim
        navigator.Stop();
        dashIndicator.SetActive(true);
        shouldAimDash = true;
        yield return new WaitForSeconds(dashAimTime);

        // lock
        shouldAimDash = false;
        yield return new WaitForSeconds(dashLockTime);

        dashIndicator.SetActive(false);
        movesUntilDash = Random.Range(dashIntervalMin, dashIntervalMax + 1);
        
        // dash
        float moveTimer = 0f;
        rb2d.velocity = dashIndicator.transform.right * dashSpeed;
        animator.Play("SlimeMove" + GetDirectionString(rb2d.velocity), -1, 0);
        Helpers.Invoke(this, () => {
            // shockwave
            Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
        }, shockwaveDelay);

        while (moveTimer < moveTime) {
            yield return new WaitForFixedUpdate();
            moveTimer += Time.deltaTime;
            rb2d.velocity = dashIndicator.transform.right * Mathf.Lerp(dashSpeed, seekSpeed / 4f, moveTimer / moveTime);
        }
        shouldPickAction = true;
    }


    private string GetDirectionString(Vector2 direction) {
        float moveAngle = Vector2.SignedAngle(Vector2.right, direction.normalized);
        int moveDir = Mathf.RoundToInt(moveAngle / 90f).Mod(4);
        return new string[] { "Right", "Up", "Left", "Down" }[moveDir];
    }


}
