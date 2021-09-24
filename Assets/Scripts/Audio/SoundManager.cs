using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager {
    //From Code Monkey Tutorial simple sound manager

    
    

    public enum Sound
    {
        spongeGun,
        Slime1,
        Slime2,
        Broom1,
        Broom,
        

    }
    public static void PlaySound(Sound sound, float SEvolume)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        var ac = GetAudioClip(sound);
        audioSource.PlayOneShot(ac, SEvolume);
        Object.Destroy(soundGameObject, ac.length);
    }


    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.i.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("Sound " + sound + " not found!");
        return null;
    }

    
}
