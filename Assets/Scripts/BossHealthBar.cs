using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BossHealthBar : MonoBehaviour
{
    BaseEnemy bossEnemy;
    public Slider slider;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI label;

    public float fadeTime;
    
    public void Init(BaseEnemy enemy, string bossName) {
        bossEnemy = enemy;
        label.text = bossName;
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1f, fadeTime).SetEase(Ease.Linear);
    }

    public void Destroy() {
        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, fadeTime).SetEase(Ease.Linear).OnComplete(() => Destroy(gameObject));
    }

    private void FixedUpdate() {
        slider.value = bossEnemy != null ? bossEnemy.health.GetHealthPercent() : 0;
    }
}
