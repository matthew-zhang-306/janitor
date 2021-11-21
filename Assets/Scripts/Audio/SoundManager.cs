using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class SoundManager {
    //From Code Monkey Tutorial simple sound manager
    private class Buffer : MonoBehaviour {
        public System.Action ResetBuffer;
        public float delay;
        public AudioClip aclip;
        public AudioSource asource;

        public bool canPlay;

        void Start ()
        {
            
            canPlay = true;
            // this.Invoke(() => gameObject.SetActive(false), aclip.length);
            // this.Invoke(() => gameObject.SetActive(false), delay);
        }
        
        public void Play (float volume)
        {
            if (canPlay) {
                this.Invoke(()=> canPlay = true, delay);
                asource.PlayOneShot(aclip, volume);
                canPlay = false;
            }
            
        }

        void OnEnable ()
        {
            canPlay = true;

        }
    }
    public enum Sound
    {
        spongeGun,
        Slime1,
        Slime2,
        Broom1,
        Dash,
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
    
    private static Dictionary<Sound, Buffer> buffer = new Dictionary<Sound, Buffer>();
    
    public static AudioSource PlaySound(Sound sound, float SEvolume)
    {
        
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        var soundMixer = Resources.Load<AudioMixer>("AudioMixer");
        var soundMixerGroup = soundMixer.FindMatchingGroups("SFX")[0]; 
        audioSource.outputAudioMixerGroup = soundMixerGroup;
        var ac = GetAudioClip(sound);
        
        audioSource.PlayOneShot(ac, SEvolume);
        Object.Destroy(soundGameObject, ac.length);

        //Can be null on access so plz check
        return audioSource;
    }

    public static void PlaySoundBuffered(Sound sound, float SEvolume, float maxRate)
    {
        //Plays Sound, but only at maxRate frequency
        if (!buffer.ContainsKey(sound)){
            var soundGameObject = new GameObject("BufferedSound");
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            var ac = GetAudioClip(sound);
            // audioSource.PlayOneShot(ac, SEvolume);
            Buffer buf = soundGameObject.AddComponent<Buffer>();

            buf.delay = maxRate;
            buf.aclip = ac;
            buf.asource = audioSource;

            buffer[sound] = buf;
            buf.Play(SEvolume);
        }
        else {
            buffer[sound].Play(SEvolume);
        }
        
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
