using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BeamTurretFiring : MonoBehaviour
{
    
    bool IsFiring;
    float maxDistance;
    public float growRate;
    int layerMask;
    SpriteRenderer spriteRenderer;
    public float disappearTime;
    Collider2D hurtBox;
    public SpriteRenderer floorMarkerSprite;
    GameObject floorMarker;

    // Start is called before the first frame update
    void Start()
    {

        layerMask = LayerMask.GetMask("Wall", "Sides");
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.size = new Vector2(spriteRenderer.size.x, 0);
        floorMarkerSprite.size = new Vector2(spriteRenderer.size.x, 0);
        hurtBox = GetComponent<Collider2D>();

        floorMarker = transform.GetChild(0).gameObject;
    }


    // Update is called once per frame
    void FixedUpdate()
    {

        if (maxDistance > 0)
        {
            var hitWall = Physics2D.Raycast(transform.position, transform.up, maxDistance, layerMask);
            if (hitWall.collider != null)
            {
                float pointHit = hitWall.distance;
                spriteRenderer.size = new Vector2(spriteRenderer.size.x, pointHit);
                floorMarkerSprite.size = new Vector2(spriteRenderer.size.x, pointHit);
            }
            else
            {
                spriteRenderer.size = new Vector2(spriteRenderer.size.x, maxDistance);
                floorMarkerSprite.size = new Vector2(spriteRenderer.size.x, maxDistance);
            }
        }

        
    }

    public void StartFiring()
    {
        IsFiring = true;
        DOTween.To(d => maxDistance = d, 0f, 20f, 20f / growRate).SetEase(Ease.Linear).SetLink(gameObject).SetTarget(this);
        hurtBox.enabled = true;
        floorMarker.SetActive(true);
    }

    public void StopFiring()
    {
        IsFiring = false;
        hurtBox.enabled = false;
        floorMarker.SetActive(false);
        transform.DOScaleX(0,disappearTime).SetEase(Ease.OutCubic).OnComplete(( ) => {
            maxDistance = 0;
            this.transform.localScale = Vector3.one;
            spriteRenderer.size = new Vector2(spriteRenderer.size.x, 0);
        }) ;
    }
}
