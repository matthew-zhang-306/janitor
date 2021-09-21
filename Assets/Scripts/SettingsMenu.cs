using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public static float SEvolume = 1f;

    public void SetVolumeMaster(float volume)
    {
        Debug.Log(volume);
        audioMixer.SetFloat("Volume", volume);
        
    }
    public void SetVolumeSE(float volumeSE)
    {
        Debug.Log(volumeSE);
        
        SEvolume = volumeSE;
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
