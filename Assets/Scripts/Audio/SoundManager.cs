using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// From comments from https://www.youtube.com/watch?v=QL29aTa7J5Q
// Probably heavily modified by the time anyone reads this
public class SoundManager : MonoBehaviour
{
    [SerializeField] protected AudioMixerGroup mixerGroup;
    [SerializeField] protected SoundCollection[] sounds;
    protected Dictionary<string, float> soundTimerDictionary = new Dictionary<string, float>();
    private HashSet<Sound> inUseSounds = new HashSet<Sound>();
    IEnumerator LoadSound(Sound sound)
    {
        sound.handle = Addressables.LoadAssetAsync<AudioClip>(sound.assetReference);
        yield return sound.handle;
        if (sound.handle.Status == AsyncOperationStatus.Succeeded)
        {
            sound.componentAudioSource = this.gameObject.AddComponent<AudioSource>();
            sound.componentAudioSource.clip = sound.handle.Result;
        }
        else
        {
            Debug.LogError("Unable to load sound " + sound.filename + " due to " +
                                sound.handle.OperationException);
            yield break;
        }
        
    }
    
    private void ResetTimer(Sound sound)
    {
        if (sound.hasCooldown)
        {
            soundTimerDictionary[GetSoundName(sound)] = int.MinValue;
        }
    }
    

    protected Sound FindSound(string soundName)
    {
        foreach (SoundCollection soundCollection in sounds)
        {
            foreach (Sound sound in soundCollection.sounds)
            {
                if (sound.filename.Equals(soundName))
                {
                    return sound;
                }
            }
        }

        Debug.LogError("Sound " + soundName + " Not Found!");
        return null;
    }

    // Play sound with name, will be created if it doesn't exist
    public IEnumerator Play(string name, bool playMultiple = false)
    {
        Sound sound = FindSound(name);
        
        if (!CanPlaySound(sound))
        {
            yield break;
        }

        yield return LoadSound(sound);
        AudioSource source = sound.componentAudioSource;
        source.outputAudioMixerGroup = mixerGroup;
        inUseSounds.Add(sound);
        // Fuck you managed code stripping
        int id = mixerGroup.GetHashCode();
        if(id > 0 || id < 1){
            source.volume = sound.volume;
        }
        
        source.pitch = sound.pitch;
        source.loop = sound.isLoop;
        source.priority = sound.priority;
        source.spatialBlend = sound.spatialBlend;

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
                sound.componentAudioSource.Play();
                sound.componentAudioSource.pitch = sound.pitch;
                sound.componentAudioSource.playOnAwake = false;
                sound.componentAudioSource.volume = 0;
                StartCoroutine(Fade(sound.componentAudioSource, sound.fadeInTime, sound.volume));
            }
        }
        
    }

    private IEnumerator Fade(AudioSource source, float duration, float finalVolume)
    {
        float startVolume = source.volume;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            source.volume = Mathf.Lerp(startVolume, finalVolume, t);
            yield return null;
        }
    }

    // Plays sound with name at location, creates a new one if it doesn't exist at absolute location. Sound temp object is deleted after being played

    public IEnumerator PlayRandom(string name, int size)
    {
        yield return Play(name.SetNumber(UnityEngine.Random.Range(1, size + 1)), true);
    }

    public void Stop(string name)
    {
        Sound sound = FindSound(name);
        if (sound.componentAudioSource != null && sound.componentAudioSource.isActiveAndEnabled)
        {
            StartCoroutine(StopAfterSeconds(sound, sound.fadeOutTime));
            StartCoroutine(Fade(sound.componentAudioSource, sound.fadeOutTime, 0));
            ResetTimer(sound);
        }

        inUseSounds.Remove(sound);
    }

    private IEnumerator StopAfterSeconds(Sound sound, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        sound.componentAudioSource.Stop();
        if (sound.releaseStrategy == Sound.ReleaseStrategy.AlsoOnStop)
        {
            Debug.Log("Stopping audio and then disposing it");
            sound.Dispose();
        }
    }

    public void OnDisable()
    {
        foreach (Sound sound in inUseSounds)
        {
            sound.Dispose();
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
        if (!sound.assetReference.RuntimeKeyIsValid())
        {
            Debug.LogWarning("Asset reference for " + sound.filename + " doesn't exist");
            return false;
        }
        
        string soundName = GetSoundName(sound);
        if (soundTimerDictionary.ContainsKey(soundName))
        {
            float lastTimePlayed = soundTimerDictionary[soundName];

            if (lastTimePlayed + sound.clipLength + sound.delay < Time.time)
            {
                soundTimerDictionary[soundName] = Time.time;
                return true;
            }

            return false;
        }

        return true;
    }
}