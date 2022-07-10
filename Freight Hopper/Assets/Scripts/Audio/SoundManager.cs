using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// From comments from https://www.youtube.com/watch?v=QL29aTa7J5Q
// Probably heavily modified by the time anyone reads this
public class SoundManager : MonoBehaviour
{
    [SerializeField] protected AudioMixerGroup mixerGroup;
    public AudioMixerGroup MixerGroup => mixerGroup;
    [SerializeField] protected SoundCollection[] sounds;
    protected Dictionary<string, float> soundTimerDictionary = new Dictionary<string, float>();

    AsyncOperationHandle switchMusicOperationHandle;
    
    protected void CreateAudioSource(Sound sound)
    {
        sound.componentAudioSource = this.gameObject.AddComponent<AudioSource>();
#if !UNITY_EDITOR
        UpdateAudioSource(sound, sound.componentAudioSource);
#endif
        if (sound.hasCooldown)
        {
            soundTimerDictionary[GetSoundName(sound)] = -100;
        }
    }

    private void ResetTimer(Sound sound)
    {
        if (sound.hasCooldown)
        {
            soundTimerDictionary[GetSoundName(sound)] = int.MinValue;
        }
    }

    protected void UpdateAudioSource(Sound sound, AudioSource source)
    {
        source.clip = sound.clip;
        source.outputAudioMixerGroup = mixerGroup;

        // Fuck you managed code stripping
        int id = mixerGroup.GetHashCode();
        if(id > 0 || id < 1){
            source.volume = sound.volume;
        }
        
        source.pitch = sound.pitch;
        source.loop = sound.isLoop;
        source.priority = sound.priority;
        source.spatialBlend = sound.spatialBlend;
    }

    protected Sound FindSound(string name)
    {
        foreach (SoundCollection soundCollection in sounds)
        {
            foreach (Sound sound in soundCollection.sounds)
            {
                if (sound.filename.Equals(name))
                {
                    if (sound.componentAudioSource == null)
                    {
                        CreateAudioSource(sound);
                    }
#if UNITY_EDITOR
                    UpdateAudioSource(sound, sound.componentAudioSource);
#endif
                    return sound;
                }
            }
        }

        Debug.LogError("Sound " + name + " Not Found!");
        return null;
    }

    // Play sound with name, will be created if it doesn't exist
    public void Play(string name, bool playMultiple = false, bool addressables = false)
    {
        Sound sound = FindSound(name);

        if (!CanPlaySound(sound))
        {
            return;
        }
        if (sound.pitchVarience > 0)
        {
            sound.componentAudioSource.pitch += UnityEngine.Random.Range(-sound.pitchVarience, sound.pitchVarience);
        }
        if (sound.volumeVarience > 0)
        {
            sound.componentAudioSource.volume += UnityEngine.Random.Range(-sound.volumeVarience, sound.volumeVarience);
        }
        if (sound.componentAudioSource.isActiveAndEnabled)
        {
            if (playMultiple)
            {
                sound.componentAudioSource.PlayOneShot(sound.componentAudioSource.clip);
                sound.componentAudioSource.pitch = sound.pitch;
                sound.componentAudioSource.playOnAwake = false;
            }
            else
            {
                if (addressables)
                {
                    StartCoroutine(PlaySongAsync(sound));
                    sound.componentAudioSource.pitch = sound.pitch;
                    sound.componentAudioSource.playOnAwake = false;
                    sound.componentAudioSource.volume = 0;
                }
                else
                {
                    sound.componentAudioSource.Play();
                    sound.componentAudioSource.pitch = sound.pitch;
                    sound.componentAudioSource.playOnAwake = false;
                    sound.componentAudioSource.volume = 0;
                    StartCoroutine(Fade(sound.componentAudioSource, sound.fadeInTime, sound.volume));
                }
                
            }
        }
    }

    private IEnumerator PlaySongAsync(Sound sound)
    {
        if (switchMusicOperationHandle.IsValid())
        {
            yield return Stop(sound.filename);
            sound.componentAudioSource.clip = null;
            sound.clip = null;
            Addressables.Release(switchMusicOperationHandle);
        }
        switchMusicOperationHandle = sound.clip2.LoadAssetAsync<AudioClip>();
        yield return switchMusicOperationHandle;
        
        sound.componentAudioSource.clip = (AudioClip)switchMusicOperationHandle.Result;
        sound.componentAudioSource.Play();
        StartCoroutine(Fade(sound.componentAudioSource, sound.fadeInTime, sound.volume));
    }
    
    private IEnumerator Fade(AudioSource source, float duration, float finalVolume, bool stop = false)
    {
        float startVolume = source.volume;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            source.volume = Mathf.Lerp(startVolume, finalVolume, t);
            yield return null;
        }
        if (stop)
        {
            source.Stop();
        }
    }

    // Plays sound with name at location, creates a new one if it doesn't exist at absolute location. Sound temp object is deleted after being played

    public void PlayRandom(string name, int size)
    {
        Play(name.SetNumber(UnityEngine.Random.Range(1, size + 1)), true);
    }

    public IEnumerator Stop(string name)
    {
        Sound sound = FindSound(name);
        if (sound.componentAudioSource.isActiveAndEnabled)
        {
            yield return Fade(sound.componentAudioSource, sound.fadeOutTime, 0, true);
            ResetTimer(sound);
        }
    }

    protected string GetSoundName(Sound sound)
    {
        string soundName = sound.filename;
        if (!string.IsNullOrEmpty(sound.groupingName))
        {
            soundName = sound.groupingName;
        }
        return soundName;
    }

    protected bool CanPlaySound(Sound sound)
    {
        string soundName = GetSoundName(sound);
        if (soundTimerDictionary.ContainsKey(soundName))
        {
            float lastTimePlayed = soundTimerDictionary[soundName];

            if (lastTimePlayed + sound.clip.length + sound.delay < Time.time)
            {
                soundTimerDictionary[soundName] = Time.time;
                return true;
            }

            return false;
        }

        return true;
    }
}