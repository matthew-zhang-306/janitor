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

    private bool hasLayers => tenseAudioSource != null || menuAudioSource != null;
    private bool isFadingOut;

    private void OnEnable() {
        RoomManager.OnEnter += OnRoomEnter;
        RoomManager.OnClear += OnRoomClear;
        PlayerController.OnRestart += OnRestart;
        PauseMenu.OnPause += OnPause;
        PauseMenu.OnResume += OnResume;
        LevelEndZone.OnLevelEnd += OnLevelEnd;
        PlaySong();
    }

    private void OnDisable() {
        RoomManager.OnEnter -= OnRoomEnter;
        RoomManager.OnClear -= OnRoomClear;
        PlayerController.OnRestart -= OnRestart;
        PauseMenu.OnPause -= OnPause;
        PauseMenu.OnResume -= OnResume;
        LevelEndZone.OnLevelEnd -= OnLevelEnd;
    }

    public void PlaySong() {
        calmAudioSource.volume = volume;
        calmAudioSource.clip = song.calmClip;
        calmAudioSource.Play();

        if (hasLayers) {
            tenseAudioSource.volume = 0;
            menuAudioSource.volume = 0;
            tenseAudioSource.clip = song.tenseClip;
            menuAudioSource.clip = song.calmMenuClip;
            tenseAudioSource.Play();
            menuAudioSource.Play();
        }

        currentAudio = calmAudioSource;
    }

    private void Update() {
        if (calmAudioSource.time > song.loopEndTime) {
            // loop everything
            calmAudioSource.time -= song.loopEndTime - song.loopStartTime;
            
            if (hasLayers) {
                tenseAudioSource.time = calmAudioSource.time;
                menuAudioSource.time = calmAudioSource.time;
            }
        }
    }


    private Tween GetFadeTween(AudioSource audioSource, bool shouldPlay, float duration) {
        Ease ease = Ease.Linear;
        if (hasLayers) {
            ease = shouldPlay ? Ease.OutCubic : Ease.InCubic;
        }

        return audioSource.DOFade(shouldPlay ? volume : 0, duration).SetEase(ease);
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
        if (!hasLayers) {
            Debug.LogError("SongPlayer " + this + " can't switch to menu audio because it has no layers");
            return;
        }

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
        if (hasLayers) {
            SwitchSource(null, 2f);
        }
        else {
            calmAudioSource.DOKill();
            GetFadeTween(calmAudioSource, false, 2f);
        }
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

    private void OnLevelEnd() {
        SwitchSource(null, 2f);
    }
}
