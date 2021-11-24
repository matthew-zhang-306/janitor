using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeIntensity;
    [SerializeField] private float shakeTime;

    private CinemachineVirtualCamera vcam;
    private CinemachineBasicMultiChannelPerlin vnoise;

    private void Start() {
        vcam = GetComponent<CinemachineVirtualCamera>();
        vnoise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        vnoise.m_AmplitudeGain = 0;
    }

    private void OnEnable() {
        PlayerController.OnTakeDamage += OnTakeDamage;
        Explosion.OnExplode += OnExplode;
    }
    private void OnDisable() {
        PlayerController.OnTakeDamage -= OnTakeDamage;
        Explosion.OnExplode -= OnExplode;
    }


    private void OnTakeDamage(PlayerController _) {
        Shake(shakeIntensity, shakeTime);
    }
    private void OnExplode() {
        Shake(shakeIntensity, shakeTime);
    }


    private void Shake(float intensity, float time) {
        this.DOKill();
        DOTween.To(v => vnoise.m_AmplitudeGain = v, intensity, 0, time)
            .SetEase(Ease.InCubic)
            .SetLink(gameObject).SetTarget(this);
    }
}
