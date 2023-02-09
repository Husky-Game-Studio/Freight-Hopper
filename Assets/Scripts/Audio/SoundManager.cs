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
    public void Play(string name, bool playMultiple = false)
    {
        if (this.gameObject.activeSelf == false) return;
        StartCoroutine(PlayAsync(name, playMultiple));
    }
    // Play sound with name, will be created if it doesn't exist
    public IEnumerator PlayAsync(string name, bool playMultiple = false)
    {
        Sound sound = FindSound(name);
        
        if (!CanPlaySound(sound))
        {
            yield break;
        }
        inUseSounds.Add(sound);
        yield return sound.LoadSound(this.gameObject);
        AudioSource source = sound.componentAudioSource;
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
                StartCoroutine(Fade(sound, sound.fadeInTime, sound.volume));
            }
        }
        if(sound.hasCooldown)
        {
            soundTimerDictionary[name] = Time.time;
        }
        if(!sound.isLoop){
            StartCoroutine(StopAfterSeconds(sound, sound.clipLength));
        }
        
    }

    private IEnumerator Fade(Sound sound, float duration, float finalVolume)
    {
        while (sound.IsLoading)
        {
            yield return null;
        }
        float startVolume = sound.componentAudioSource.volume;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            sound.componentAudioSource.volume = Mathf.Lerp(startVolume, finalVolume, t);
            yield return null;
        }
    }

    // Plays sound with name at location, creates a new one if it doesn't exist at absolute location. Sound temp object is deleted after being played

    public IEnumerator PlayRandom(string name, int size)
    {
        yield return PlayAsync(name.SetNumber(UnityEngine.Random.Range(1, size + 1)), true);
    }

    public void Stop(string name, bool clearInUseSound = true)
    {
        Sound sound = FindSound(name);
        if (sound.componentAudioSource != null && sound.componentAudioSource.isActiveAndEnabled)
        {
            StartCoroutine(StopAfterSeconds(sound, sound.fadeOutTime));
            StartCoroutine(Fade(sound, sound.fadeOutTime-Time.deltaTime*2, 0));
            if(clearInUseSound){
                inUseSounds.Remove(sound);
            }
        }
    }
    public void StopAll(){
        foreach (Sound sound in inUseSounds)
        {
            Stop(sound.filename, false);
        }
        inUseSounds.Clear();
    }
    private IEnumerator StopAfterSeconds(Sound sound, float seconds)
    {
        if(seconds > float.Epsilon){
            yield return new WaitForSeconds(seconds);
        }
        
        while (sound.IsLoading)
        {
            yield return null;
        }
        
        sound.componentAudioSource.Stop();
        sound.Dispose();
        if (sound.hasCooldown)
        {
            soundTimerDictionary[GetSoundName(sound)] = int.MinValue;
        }
    }

    public void OnDisable()
    {
        foreach (Sound sound in inUseSounds)
        {
            sound.Dispose();
        }
        inUseSounds.Clear();
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
        if (sound.IsLoading)
        {
            return false;
        }
        if (!sound.assetReference.RuntimeKeyIsValid())
        {
            Debug.LogWarning("Asset reference for " + sound.filename + " doesn't exist");
            return false;
        }
        
        string soundName = GetSoundName(sound);
        if (soundTimerDictionary.ContainsKey(soundName))
        {
            float lastTimePlayed = soundTimerDictionary[soundName];
            
            if (lastTimePlayed + sound.clipLength < Time.time)
            {
                soundTimerDictionary[soundName] = Time.time;
                return true;
            }
            return false;
        }

        return true;
    }
}