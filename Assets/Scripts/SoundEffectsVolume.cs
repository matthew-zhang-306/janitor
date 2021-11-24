using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundEffectsVolume : MonoBehaviour
{
    
    public Slider seVolume;
    public AudioMixer audioMixer;

    public void VolumeUpdate(float volume)
    {
        volume = seVolume.value;
        audioMixer.SetFloat("SEVolume", volume);
        
    }
}
