using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpriteFlash : MonoBehaviour
{
    public SpriteRenderer originalSprite;
    private SpriteRenderer flashSprite;
    private Animator animator;
    private Transform visualsContainer;

    public float tiltAngle = 30f;
    public float tiltTime = 0.3f;

    private bool oldIsBlinking;
    public bool IsBlinking { get; private set; }

    private void Start() {
        flashSprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        visualsContainer = transform.parent;
    }

    private void LateUpdate() {
        flashSprite.sprite = originalSprite.sprite;
        flashSprite.flipX = originalSprite.flipX;
        flashSprite.flipY = originalSprite.flipY;
        
        flashSprite.transform.localPosition = originalSprite.transform.localPosition;
        flashSprite.transform.localRotation = originalSprite.transform.localRotation;
        flashSprite.transform.localScale = originalSprite.transform.localScale;

        if (IsBlinking) {
            originalSprite.color = originalSprite.color.WithAlpha(flashSprite.color.a + 0.25f);
        }
        else if (oldIsBlinking) {
            originalSprite.color = originalSprite.color.WithAlpha(1);
        }
        oldIsBlinking = IsBlinking;
    }


    public void Flash(float time, float tiltDirection, float timeUntilBlink = 0.15f) {
        this.DOKill();
        visualsContainer.localRotation = Quaternion.Euler(0, 0, tiltAngle * Mathf.Sign(tiltDirection));

        DOTween.Sequence()
            .Append(visualsContainer.DOLocalRotate(Vector3.zero, tiltTime).SetEase(Ease.OutBack))
            .InsertCallback(0, () => animator.Play("Flash"))
            .InsertCallback(Mathf.Min(time, timeUntilBlink), () => {
                animator.Play("FlashBlink");
                IsBlinking = true;
            })
            .InsertCallback(time, () => {
                animator.Play("FlashStart");
                IsBlinking = false;
            })
            .SetTarget(this).SetLink(gameObject);
    }

}
