using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu()]
public class Song : ScriptableObject
{
    public AudioClip calmClip;
    public AudioClip tenseClip;
    public AudioClip calmMenuClip;
    public AudioClip tenseMenuClip;
    public float loopStartTime;
    public float loopEndTime;
}
