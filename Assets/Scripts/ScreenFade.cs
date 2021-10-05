using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScreenFade : MonoBehaviour
{
    private Image image;
    public float fadeTime;

    private void Start() {
        image = GetComponent<Image>();
        image.color = new Color(0, 0, 0, 1);
        FadeIn();
    }

    private void OnEnable() {
        PlayerController.OnDeath += FadeOutReset;
        PlayerController.OnRestart += FadeInReset;
    }

    private void OnDisable() {
        PlayerController.OnDeath -= FadeOutReset;
        PlayerController.OnRestart -= FadeInReset;
    }


    public void FadeIn() {
        image.DOKill();
        image.DOFade(0, fadeTime);
    }

    public void FadeOut() {
        image.DOKill();
        image.DOFade(1, fadeTime);
    }

    private void FadeOutReset(PlayerController _) {
        Helpers.Invoke(this, FadeOut, 2f - fadeTime);
    }

    private void FadeInReset(PlayerController _) {
        FadeIn();   
    }
}
