using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySongTrigger : MonoBehaviour
{
    public Song song;
    public bool shouldPlayOnStart;
    public bool shouldTriggerOnlyOnce;

    private bool isActive = true;

    private void Start() {
        if (shouldPlayOnStart) {
            GameObject.FindGameObjectWithTag("SongPlayer").GetComponent<SongPlayer>().PlaySong(song);

            if (shouldTriggerOnlyOnce)
                isActive = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (isActive && other.CompareTag("Player")) {
            SongPlayer songPlayer = GameObject.FindGameObjectWithTag("SongPlayer").GetComponent<SongPlayer>();
            if (songPlayer.IsPlaying()) {
                songPlayer.SwitchSongWithTransition(song);
            } else {
                songPlayer.PlaySong(song);
            }

            if (shouldTriggerOnlyOnce)
                isActive = false;
        }
    }
}
