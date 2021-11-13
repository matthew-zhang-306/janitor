using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using DG.Tweening;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class SlimeEnemy : BaseEnemy
{
    public float agroRadius;
    public float seekSpeed;
    public AudioSource slimeMove; //0915BR
    public AudioClip[] sounds; //0915

    [SerializeField] private float actionTimer = 1.5f;
    private float m_time;

    [SerializeField] private float dirtyTime = 1f;
    
    protected override void Start()
    {
        base.Start();

        m_time = Random.Range(0f, actionTimer);
        DOTween.Sequence()
            .InsertCallback(0f, () => floorMarker.SetActive(true))
            .InsertCallback(0.2f, () => floorMarker.SetActive(false))
            .InsertCallback(dirtyTime, () => floorMarker.SetActive(true))
            .SetLoops(-1, LoopType.Restart)
            .SetLink(gameObject);
    }  
    


    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (CanAct && m_time > actionTimer) {
            m_time -= actionTimer;

            navigator.canNavigate = false;

            // chose action
            if (player != null &&
                Vector3.Distance(player.transform.position, this.transform.position) <= agroRadius)
            {
                Action_Move();
            } else {
                Action_Wander();
            }
        }
        m_time += Time.deltaTime;
    }


    //Action Methods should only be called from itself
    protected virtual void Action_Wander ()
    {
        navigator.canNavigate = false;
        Vector3 randomDir = Quaternion.Euler(0, 0, Random.Range(0f, 360f)) * Vector3.right;
        rb2d.AddForce(randomDir * 30 * rb2d.mass);

        animator.Play("SlimeIdle");
    }

    protected virtual void Action_Move ()
    {
        navigator.SetDestination(player.transform.position, () => {
            PlayRandom();
        
            navigator.DOKill();
            navigator.speed = seekSpeed;
            DOTween.Sequence()
                .Append(DOTween.To(() => navigator.speed, s => navigator.speed = s, seekSpeed / 4f, actionTimer))
                .InsertCallback(0.05f, SelectMoveAnimation)
                .SetLink(gameObject).SetTarget(navigator);

            // the reason we have to delay selecting the move animation for 0.05 seconds is that
            // very often the navigator finds a path that goes in one direction for a very short distance
            // and then turns another direction, like so:
            //   X
            //   |------> (1 cell down, then 7 cells right)
            // here if we didn't use a small delay, the slime would face straight down even though it
            // is primarily moving to the right
        });
        
    }
    

    private void SelectMoveAnimation() {
        float moveAngle = Vector2.SignedAngle(Vector2.right, rb2d.velocity);
        int moveDir = Mathf.RoundToInt(moveAngle / 90f).Mod(4);
        string moveString = new string[] { "Right", "Up", "Left", "Down" }[moveDir];

        // yes! you have to specify the two other parameters -1 and 0.
        // for some reason animator.Play(clip) works COMPLETELY DIFFERENTLY from
        // animator.Play(clip, number, number) if you try to play an animation
        // that the animator is already playing. it's extremely dumb
        animator.Play("SlimeMove" + moveString, -1, 0);
    }


    protected override void TakeDamage(Collider2D other) {
        base.TakeDamage(other);

        float per = (1 - health.GetHealthPercent())/2 + health.GetHealthPercent();
        this.transform.localScale = Vector3.one * per;
    }

    private void PlayRandom() { //0915BR
        
        slimeMove.clip = sounds[Random.Range(0, sounds.Length)];
        slimeMove.Play();       
    }  
}