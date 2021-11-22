using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class BossSongTrigger : MonoBehaviour
{
    private float songVolume;
    SongPlayer bossSongPlayer;

    public SongPlayer normalSongPlayer;
    public Checkpoint bossCheckpoint;
    public RoomManager bossRoom;

    public AudioMixer audioMixer;
    private bool inBossRoom;
    private bool atBossCheckpoint;

    private void Awake() {
        bossSongPlayer = GetComponent<SongPlayer>();
    }

    private void Start() {
        SetVolume("levelVolume", 1f);
        SetVolume("bossVolume", 0.0001f);
    }

    private void OnEnable() {
        bossCheckpoint.OnCheckpointExit += OnCheckpointExit;
        RoomManager.OnEnter += OnRoomEnter;
        RoomManager.OnClear += OnRoomClear;
        PlayerController.OnDeath += OnDeath;
    }
    private void OnDisable() {
        bossCheckpoint.OnCheckpointExit -= OnCheckpointExit;
        RoomManager.OnEnter -= OnRoomEnter;
        RoomManager.OnClear -= OnRoomClear;
        PlayerController.OnDeath -= OnDeath;
    }


    private void OnCheckpointExit(Checkpoint checkpoint, Transform playerTransform) {
        if (Vector3.Dot(
                playerTransform.position - checkpoint.transform.position,
                checkpoint.transform.right
            ) > 0)
        {
            this.DOKill();
            DoFade("bossVolume", 0.0001f, 0.7f);
            DoFade("levelVolume", 0.0001f, 5.0f);
        }
        else {
            // e
            this.DOKill();
            DoFade("bossVolume", 0.0001f, 0.7f);
            DoFade("levelVolume", 1f, 1.0f);
        }
    }

    private void OnRoomEnter(PlayerController _, RoomManager roomManager) {
        if (roomManager != bossRoom)
            return;

        inBossRoom = true;
        bossSongPlayer.RestartSong();

        this.DOKill();
        SetVolume("bossVolume", 1f);
        DoFade("levelVolume", 0.0001f, 0.5f);
    }

    private void OnRoomClear(PlayerController _, RoomManager roomManager) {
        if (roomManager != bossRoom)
            return;
        
        inBossRoom = false;
        DoFade("bossVolume", 0.0001f, 5.0f);
    }

    private void OnDeath(PlayerController _) {
        if (!inBossRoom)
            return;
        
        inBossRoom = false;
        DoFade("bossVolume", 0.0001f, 5.0f);
    }


    public void DoFade(string paramName, float value, float time) {
        DOTween.To(
            () => GetVolume(paramName),
            v => SetVolume(paramName, v),
            value,
            time
        ).SetEase(Ease.Linear)
        .SetTarget(this).SetLink(gameObject);
    }

    public float GetVolume(string paramName) {
        audioMixer.GetFloat(paramName, out float value);
        return Helpers.DBToVolume(value);
    }
    public void SetVolume(string paramName, float value) {
        audioMixer.SetFloat(paramName, Helpers.VolumeToDB(value));
    }

}
