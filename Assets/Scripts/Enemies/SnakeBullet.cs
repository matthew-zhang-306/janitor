using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SnakeBullet : MonoBehaviour
{
    private Vector3 targetPos;

    [SerializeField] private float travelTime = default;
    [SerializeField] private float travelHeight = default;
    private bool isLanding;

    [Header("Objects")]
    [SerializeField] private GameObject explosionPrefab = default;
    [SerializeField] private Transform indicatorTransform = default;
    [SerializeField] private Transform shadowTransform = default; // positioned horizontally across the floor
    [SerializeField] private Transform projectileTransform = default; // positioned above the shadow based on height
    [SerializeField] private Hitbox projectileHitbox = default;
    [SerializeField] private SpriteRenderer projectileSprite = default;


    public void SetTarget(Vector3 _targetPos) {
        targetPos = _targetPos;
        
        this?.DOKill();
        indicatorTransform.position = targetPos;
        shadowTransform.position = transform.position;
        projectileTransform.position = transform.position;
        DOTween.Sequence()
            .Insert(0, projectileTransform.DOLocalMoveY(travelHeight, travelTime / 2f).SetEase(Ease.OutQuad))
            .Insert(travelTime / 2f, projectileTransform.DOLocalMoveY(0, travelTime / 2f).SetEase(Ease.InQuad))
            .Insert(0, shadowTransform.DOMove(targetPos, travelTime).SetEase(Ease.Linear))
            .InsertCallback(travelTime * 0.8f, () => isLanding = true) // set isLanding bool in the final parts of the motion
            .InsertCallback(travelTime, Finish)
            .SetLink(gameObject).SetTarget(this);
    }

    private void FixedUpdate() {
        if (isLanding && projectileHitbox.IsColliding) {
            // hit the player: strike early
            Finish();
        }
    }

    private void Finish() {
        if (!isLanding)
            return;
        isLanding = false;

        this?.DOKill();

        Instantiate(explosionPrefab, targetPos, Quaternion.identity);
        gameObject.SetActive(false);
    }

    private void OnDisable() {
        // until we get this object pooled, desstroy it
        Destroy(gameObject);
    }
}
