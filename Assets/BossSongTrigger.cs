using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSongTrigger : MonoBehaviour
{
    private float songVolume;
    SongPlayer bossSongPlayer;

    public SongPlayer normalSongPlayer;
    public Checkpoint bossCheckpoint;
    public RoomManager bossRoom;

    private void Awake() {
        // songVolume = normalSongPlayer.volume;
        // bossSongPlayer.volume = 0;
    }

    private void OnEnable() {
        // bossCheckpoint.OnCheckpointExit += OnCheckpointExit;
        // RoomManager.OnEnter += OnRoomEnter;
        // RoomManager.OnClear += OnRoomClear;
        // PlayerController.OnDeath += OnDeath;
    }
    private void OnDisable() {
        // bossCheckpoint.OnCheckpointExit -= OnCheckpointExit;
        // RoomManager.OnEnter -= OnRoomEnter;
        // RoomManager.OnClear -= OnRoomClear;
        // PlayerController.OnDeath -= OnDeath;
    }


    
}
