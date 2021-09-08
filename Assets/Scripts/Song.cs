using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu()]
public class Song : ScriptableObject
{
    public AudioClip song;
    public float songStartTime;
    
    [Header("Leave these values at 0 to have the song not loop")]
    public float loopStartTime;
    public float loopEndTime;

    [Header("Will be used for transitions between songs")]
    public float measuresPerMinute;
}
