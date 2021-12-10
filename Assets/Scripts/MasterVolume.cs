using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MasterVolume : MonoBehaviour
{
    private static readonly string FirstPlay = "FirstPlay";
    private static readonly string BackgroundPref = "BackgroundPref";
    private static readonly string SoundEffectsPref = "SoundEffectsPref";
    private int firstPlayInt;
    public Slider backgroundSlider, soundEffectsSlider;
    private float backgroundFloat, soundEffectsFloat;
    public AudioSource[] backgroundAudio;
    public AudioSource[] soundEffectsAudio;
    [SerializeField] private AudioMixer audioMixer;

    public void UpdateSound(float value)
    {
        audioMixer.SetFloat("sfxVolume", Helpers.VolumeToDB(value));
    }

    public void UpdateMusic(float value)
    {
        audioMixer.SetFloat("musicVolume", Helpers.VolumeToDB(value));
    }
}
