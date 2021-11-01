using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
public class RatEnemy : BaseEnemy
{
    private bool shouldPickAction;

    public AudioSource ratAttack;
    public AudioSource ratMove;
    public AudioClip[] ratMovements;

    public float seekSpeed;
    public float stoppingDistance;
    public float startupTime;
    private float startupTimer;
    public float swipeDelay;
    public float swipeTime;

    public GameObject swipe;

    private bool m_rat = true;

    protected override void Start() {
        base.Start();

        navigator.speed = seekSpeed;
        startupTimer = UnityEngine.Random.Range(startupTime / 2f, startupTime);
        swipe.SetActive(false);
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

            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Rat" + moveString) {
                animator.Play("Rat" + moveString, 0, 0f);
            }
        }
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if (CanAct && shouldPickAction) {
            string selectedAction = "";

            if (startupTimer > 0) {
                selectedAction = "Wait";
            }
            else if ((player.transform.position - transform.position).magnitude <= stoppingDistance)
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
    }


    private IEnumerator Action_Wait() {
        navigator.Stop();

        animator.Play("RatIdle");
       
        
        while (startupTimer != 0) {
            yield return 0;
            startupTimer = Mathf.MoveTowards(startupTimer, 0, Time.deltaTime); ratMove.Stop();
        }
        shouldPickAction = true;
    }

    private IEnumerator Action_Seek() {
        navigator.canNavigate = true;
        navigator.SetDestination(player.transform.position);
        
        if (m_rat == true) { ratMove.clip = ratMovements[0]; ratMove.Play(); m_rat = false; }
        else PlayRandom();


        float timer = 0.5f;
        while (timer > 0) {     
            yield return new WaitForFixedUpdate();

            if ((player.transform.position - transform.position).magnitude <= stoppingDistance)
                // terminate immediately
                timer = 0;
          
            timer -= Time.deltaTime;
        }
        
        shouldPickAction = true;
    }

    private IEnumerator Action_Swipe() {
        navigator.Stop();

        animator.Play("RatCharge");

        yield return new WaitForSeconds(swipeDelay);

        float swipeAngle = Vector2.SignedAngle(Vector2.right, player.transform.position - transform.position);
        swipe.transform.rotation = Quaternion.Euler(0, 0, swipeAngle);
        swipe.SetActive(true);

        animator.Play("RatAttack");
        ratAttack.Play();
        Debug.Log("RatAttack!");

        yield return new WaitForSeconds(swipeTime);

        swipe.SetActive(false);
        startupTimer = startupTime;
        shouldPickAction = true;

        animator.Play("RatIdle");
    }

    private void PlayRandom() {
        ratMove.clip = ratMovements[UnityEngine.Random.Range(0, ratMovements.Length)];
        ratMove.Play();
    }
}
