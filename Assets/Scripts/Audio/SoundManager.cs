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
        Quad,
        Med,
        Turret,
        Diaper,
        Kitty,
        Snake,
        Damage,
        Walk,
        SlimeBarrel,
        Coin,
        BunnyAttack,
        Barrel,
        EnemySpawn,
        EnemyDamage,
        AmmoAlert,
        PlayerDeath,
        WaterBarrel,
        KeyCollecting,
        MouseClick,
    }
    public static void PlaySound(Sound sound, float SEvolume)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        var ac = GetAudioClip(sound);
        audioSource.PlayOneShot(ac, SEvolume);
        Object.Destroy(soundGameObject, ac.length);
    }

    public static void PlaySoundBroom(Sound sound, Sound sound2, float SEvolume) {
        Sound[] list = { sound, sound2 };
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        var number = Random.Range(0, list.Length);
        var ac = GetAudioClip(list[number]);
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

   private static IEnumerator Wait() {
        yield return new WaitForSeconds(1f);
    }
}
