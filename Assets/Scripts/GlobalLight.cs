using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using DG.Tweening;

public class GlobalLight : MonoBehaviour
{
    private new Light2D light;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color dimColor = Color.gray;
    [SerializeField] private Color flashColor = Color.white;

    [SerializeField] float dimTime = 0f;

    private void Start() {
        light = GetComponent<Light2D>();
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

    private void OnRoomEnter(PlayerController _, RoomManager __) {
        light.DOKill();
        DOTween.To(
            () => light.color,
            color => light.color = color,
            dimColor,
            dimTime
        ).SetTarget(light).SetLink(gameObject);
    }

    private void OnRoomClear(PlayerController _, RoomManager __) {
        light.DOKill();
        light.color = flashColor;
        DOTween.To(
            () => light.color,
            color => light.color = color,
            normalColor,
            dimTime
        ).SetTarget(light).SetLink(gameObject);
    }

    private void OnRestart(PlayerController _) {
        light.DOKill();
        light.color = normalColor;
    }
}
