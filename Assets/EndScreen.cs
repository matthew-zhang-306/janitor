using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EndScreen : MonoBehaviour
{
    public SpriteRenderer titleSprite;
    public CanvasGroup creditsCanvas;
    public Button advanceButton;
    bool hasAdvanced = false;
    float timer = 0;

    private void Start() {
        creditsCanvas.alpha = 0;
        creditsCanvas.interactable = false;
        creditsCanvas.blocksRaycasts = false;
    }

    private void Update() {
        timer += Time.deltaTime;
        if (timer > 10f && !hasAdvanced) {
            OnAdvance();
        }
    }

    public void OnAdvance() {
        if (hasAdvanced)
            return;
        
        hasAdvanced = true;
        advanceButton.gameObject.SetActive(false);
        titleSprite.DOFade(0, 1f).SetEase(Ease.Linear);
        creditsCanvas.DOFade(1, 1f).SetEase(Ease.Linear);
        creditsCanvas.interactable = true;
        creditsCanvas.blocksRaycasts = true;
    }
}
