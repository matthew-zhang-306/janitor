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

    public Animator animator;
    
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
    void Update()
    {
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

    protected override void FixedUpdate() {
        base.FixedUpdate();

        spriteRenderer.color = invincibilityTimer > 0 ? new Color(1, 0.4f, 0.4f) : Color.white;
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
            DOTween.To(() => navigator.speed, s => navigator.speed = s, seekSpeed / 4f, actionTimer)
                .SetLink(gameObject).SetTarget(navigator);
            
            // select animation
            float moveAngle = Vector2.SignedAngle(Vector2.right, navigator.GetMoveDirection());
            int moveDir = Mathf.RoundToInt(moveAngle / 90f).Mod(4);
            string moveString = new string[] { "Right", "Up", "Left", "Down" }[moveDir];
            Debug.Log("selecting animation " + navigator.GetMoveDirection());

            // i have no idea what the "0f" parameter in this does. all i know is that it doesn't work without it
            animator.Play("SlimeMove" + moveString, -1, 0f);
        });
        
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