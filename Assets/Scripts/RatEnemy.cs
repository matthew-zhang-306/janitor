using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
public class RatEnemy : BaseEnemy
{
    private bool shouldPickAction;

    public float seekSpeed;
    public float stoppingDistance;
    public float startupTime;
    private float startupTimer;
    public float swipeDelay;
    public float swipeTime;

    public GameObject swipe;


    protected override void Start() {
        base.Start();

        navigator.speed = seekSpeed;
        startupTimer = UnityEngine.Random.Range(startupTime / 2f, startupTime);
        swipe.SetActive(false);
        shouldPickAction = true;
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if (CanAct && shouldPickAction) {
            string selectedAction = "";

            if (startupTimer > 0) {
                selectedAction = "Wait";
            }
            else if ((player.transform.position - transform.position).sqrMagnitude <=
                        stoppingDistance * stoppingDistance * 1.1f)
            {
                selectedAction = "Swipe";
            }
            else {
                selectedAction = "Seek";
            }

            shouldPickAction = false;

            //If action was specified in inspector then go ahead and do the action
            if (actionTable.ContainsKey(selectedAction)) {
                StartCoroutine(actionTable[selectedAction]());
            }
            
        }
        else if (!CanAct) {
            // set up a startup time if the rat is set to act again later
            if (startupTimer == 0)
                startupTimer = UnityEngine.Random.Range(startupTime / 2f, startupTime);
        }

        navigator.speed = invincibilityTimer == 0 ? seekSpeed : seekSpeed / 3f;
        spriteRenderer.color = invincibilityTimer > 0 ? new Color(1, 0.4f, 0.4f) : Color.white;
    }

    protected override void TakeDamage(Collider2D other) {
        base.TakeDamage(other);
        float per = (1 - health.GetHealthPercent())/2 + health.GetHealthPercent();
    }


    private IEnumerator Action_Wait() {
        navigator.Stop();
        
        while (startupTimer != 0) {
            yield return 0;
            startupTimer = Mathf.MoveTowards(startupTimer, 0, Time.deltaTime);
        }
        shouldPickAction = true;
    }

    private IEnumerator Action_Seek() {
        navigator.canNavigate = true;
        navigator.SetDestination(player.transform.position);
        yield return 0;
        shouldPickAction = true;
    }

    private IEnumerator Action_Swipe() {
        navigator.Stop();

        yield return new WaitForSeconds(swipeDelay);

        float swipeAngle = Vector2.SignedAngle(Vector2.right, player.transform.position - transform.position);
        swipe.transform.rotation = Quaternion.Euler(0, 0, swipeAngle);
        swipe.SetActive(true);

        // play some animation here

        yield return new WaitForSeconds(swipeTime);

        swipe.SetActive(false);
        startupTimer = startupTime;
        shouldPickAction = true;
    }
}
