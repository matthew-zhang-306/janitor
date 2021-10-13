using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SongPlayer : MonoBehaviour
{
    [SerializeField] private Song song = default;
    [SerializeField] private AudioSource calmAudioSource = default;
    [SerializeField] private AudioSource tenseAudioSource = default;
    [SerializeField] private AudioSource menuAudioSource = default;
    [SerializeField] private float volume;
    private AudioSource currentAudio;

    private bool isFadingOut;

    private void OnEnable() {
        RoomManager.OnEnter += OnRoomEnter;
        RoomManager.OnClear += OnRoomClear;
        PlayerController.OnRestart += OnRestart;
        PauseMenu.OnPause += OnPause;
        PauseMenu.OnResume += OnResume;
        PlaySong();
    }

    private void OnDisable() {
        RoomManager.OnEnter -= OnRoomEnter;
        RoomManager.OnClear -= OnRoomClear;
        PlayerController.OnRestart -= OnRestart;
        PauseMenu.OnPause -= OnPause;
        PauseMenu.OnResume -= OnResume;
    }

    public void PlaySong() {
        calmAudioSource.volume = volume;
        tenseAudioSource.volume = 0;
        menuAudioSource.volume = 0;

        calmAudioSource.clip = song.calmClip;
        tenseAudioSource.clip = song.tenseClip;
        menuAudioSource.clip = song.calmMenuClip;

        calmAudioSource.Play();
        tenseAudioSource.Play();
        menuAudioSource.Play();

        currentAudio = calmAudioSource;
    }

    private void Update() {
        if (calmAudioSource.time > song.loopEndTime) {
            // loop everything
            calmAudioSource.time -= song.loopEndTime - song.loopStartTime;
            tenseAudioSource.time = calmAudioSource.time;
            menuAudioSource.time = calmAudioSource.time;
        }
    }


    private Tween GetFadeTween(AudioSource audioSource, bool shouldPlay, float duration) {
        return audioSource.DOFade(shouldPlay ? volume : 0, duration).SetEase(shouldPlay ? Ease.OutQuad : Ease.InQuad);
    }

    private void SwitchSource(AudioSource desired, float duration) {
        this.DOKill();
        DOTween.Sequence()
            .Insert(0, GetFadeTween(calmAudioSource, calmAudioSource == desired, duration))
            .Insert(0, GetFadeTween(tenseAudioSource, tenseAudioSource == desired, duration))
            .Insert(0, GetFadeTween(menuAudioSource, menuAudioSource == desired, duration))
            .SetUpdate(UpdateType.Normal, true)
            .SetLink(gameObject).SetTarget(this);
        currentAudio = desired;
    }

    private void SetMenuAudio() {
        if (currentAudio == calmAudioSource) {
            menuAudioSource.clip = song.calmMenuClip;
        }
        else {
            menuAudioSource.clip = song.tenseMenuClip;
        }
        menuAudioSource.Play();
        menuAudioSource.time = calmAudioSource.time;
    }

    public void StopSong() {
        SwitchSource(null, 2f);
    }


    private void OnRoomEnter(PlayerController _, RoomManager __) {
        SwitchSource(tenseAudioSource, 1f);
    }

    private void OnRoomClear(PlayerController _, RoomManager __) {
        SwitchSource(calmAudioSource, 1f);
    }

    private void OnRestart(PlayerController _) {
        SwitchSource(calmAudioSource, 2f);
    }

    private void OnPause() {
        Debug.Log("song pause");
        SetMenuAudio();
        SwitchSource(menuAudioSource, 0.5f);
    }

    private void OnResume() {
        SwitchSource(menuAudioSource.clip == song.calmMenuClip ? calmAudioSource : tenseAudioSource, 0.5f);
    }
}
