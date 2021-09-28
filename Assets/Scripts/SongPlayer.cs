using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SongPlayer : MonoBehaviour
{
    private Dictionary<Song, AudioSource> songAudioSources;

    private void Awake() {
        songAudioSources = new Dictionary<Song, AudioSource>();
    }

    public void PlaySong(Song song, bool cutIntro = false) {
        if (songAudioSources.ContainsKey(song)) {
            Debug.LogWarning("Song " + song.song + " is already playing!");
            return;
        }

        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        source.volume = 0.6f;
        source.clip = song.song;
        if (cutIntro)
            source.time = song.songStartTime;
        
        songAudioSources.Add(song, source);
        source.Play();
    }

    private void Update() {
        List<Song> delete = new List<Song>();
        foreach (var song in songAudioSources.Keys) {
            AudioSource audioSource = songAudioSources[song];
            if (song.loopEndTime > song.loopStartTime && audioSource.time > song.loopEndTime) {
                audioSource.time -= song.loopEndTime - song.loopStartTime;
            }
            else if (!audioSource.isPlaying) {
                delete.Add(song);
                
            }
        }

        foreach (var song in delete) {
            AudioSource audio = songAudioSources[song];
            songAudioSources.Remove(song);
            Destroy(audio);
        }
    }

    public bool IsPlaying() {
        return songAudioSources.Count != 0;
    }

    public void SwitchSongWithTransition(Song newSong) {
        if (songAudioSources.Count == 0) {
            Debug.LogWarning("No song to transition from!");
            PlaySong(newSong);
            return;
        }
        else if (songAudioSources.ContainsKey(newSong)) {
            Debug.LogWarning("Song " + newSong.song + " is already playing!");
            return;
        }

        // get the first song that is currently playing and use that one
        var songEnum = songAudioSources.GetEnumerator();
        songEnum.MoveNext();
        Song oldSong = songEnum.Current.Key;

        // figure out at what time we should start playing the new song over the old song
        float currentTime = songAudioSources[oldSong].time;
        float transitionInterval = 4 / oldSong.measuresPerMinute;
        float nextPossibleTransitionTime = Mathf.Max(currentTime, oldSong.loopStartTime) + newSong.songStartTime - oldSong.loopStartTime;
        float nextValidTransitionTime = Mathf.Ceil(nextPossibleTransitionTime / transitionInterval) * transitionInterval + oldSong.loopStartTime - newSong.songStartTime;

        // set it up to happen at the right time
        DOTween.Sequence().InsertCallback(nextValidTransitionTime - currentTime, () => {
            PlaySong(newSong);
            songAudioSources[newSong].time = Mathf.Max(songAudioSources[oldSong].time - nextValidTransitionTime, 0);
        }).InsertCallback(nextValidTransitionTime + newSong.songStartTime - currentTime, () => {
            StopSongImmediate(oldSong);
        }).SetLink(gameObject);
    }

    public void StopSong(Song song) {
        if (!songAudioSources.ContainsKey(song) || !songAudioSources[song].isPlaying) {
            Debug.LogWarning("Song " + song.song + " is not playing!");
            return;
        }
        
        // set up a fadeout
        AudioSource audioSource = songAudioSources[song];
        audioSource.DOFade(0f, 1f)
            .OnComplete(() => Destroy(audioSource));
        songAudioSources.Remove(song);
    }

    public void StopSongImmediate(Song song) {
        if (!songAudioSources.ContainsKey(song) || !songAudioSources[song].isPlaying) {
            Debug.LogWarning("Song " + song.song + " is not playing!");
            return;
        }

        AudioSource audioSource = songAudioSources[song];
        Destroy(audioSource);
        songAudioSources.Remove(song);
    }
}
