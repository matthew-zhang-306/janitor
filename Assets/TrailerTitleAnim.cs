using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TrailerTitleAnim : MonoBehaviour
{
    public Image flash;
    public Image grimeSpree;
    public RectTransform text;

    private void Start() {
        text.gameObject.SetActive(false);

        Helpers.Invoke(this, () => {
            flash.color = Color.white;
            flash.DOFade(0, 0.8f).SetEase(Ease.Linear);
            grimeSpree.rectTransform.anchoredPosition += new Vector2(0, 120);
            grimeSpree.rectTransform.DOAnchorPosY(grimeSpree.rectTransform.anchoredPosition.y - 120, 2f).SetEase(Ease.OutBack);
        }, 1f);

        Helpers.Invoke(this, () => {
            text.gameObject.SetActive(true);
            grimeSpree.gameObject.SetActive(false);
        }, 6f);
    }
}
