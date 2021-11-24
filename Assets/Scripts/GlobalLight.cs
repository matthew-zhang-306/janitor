using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using DG.Tweening;

public class GlobalLight : MonoBehaviour
{
    private new Light2D light;

    [SerializeField] private float normalIntensity = default;
    [SerializeField] private float dimIntensity = default;
    [SerializeField] private float flashIntensity = default;

    [SerializeField] float dimTime = 0f;

    private void Start() {
        light = GetComponent<Light2D>();
        light.intensity = normalIntensity;
    }

    private void OnEnable() {
        RoomManager.OnEnter += OnRoomEnter;
        RoomManager.OnClear += OnRoomClear;
        PlayerController.OnRestart += OnRestart;
    }
    private void OnDisable() {
        RoomManager.OnEnter -= OnRoomEnter;
        RoomManager.OnClear -= OnRoomClear;
        PlayerController.OnRestart -= OnRestart;
    }

    private void OnRoomEnter(PlayerController _, RoomManager room) {
        if (room.IsLeavable)
            return;
        
        light.DOKill();
        DOTween.To(
            v => light.intensity = v,
            light.intensity,
            dimIntensity,
            dimTime
        ).SetTarget(light).SetLink(gameObject);
    }

    private void OnRoomClear(PlayerController _, RoomManager room) {
        light.DOKill();
        DOTween.To(
            v => light.intensity = v,
            flashIntensity,
            normalIntensity,
            dimTime
        ).SetTarget(light).SetLink(gameObject);
    }

    private void OnRestart(PlayerController _) {
        light.DOKill();
        light.intensity = normalIntensity;
    }
}
