using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

// From comments from https://www.youtube.com/watch?v=QL29aTa7J5Q
// Probably heavily modified by the time anyone reads this
public class SoundManager : MonoBehaviour
{
    [SerializeField] protected AudioMixerGroup mixerGroup;
    [SerializeField] protected SoundCollection[] sounds;
    protected Dictionary<string, float> soundTimerDictionary = new Dictionary<string, float>();

    protected void CreateAudioSource(Sound sound)
    {
        sound.componentAudioSource = this.gameObject.AddComponent<AudioSource>();
#if !UNITY_EDITOR
        UpdateAudioSource(sound);
#endif
        if (sound.hasCooldown)
        {
            soundTimerDictionary[GetSoundName(sound)] = -100;
        }
    }

    protected void CreateAudioSource(Sound sound, Vector3 location)
    {
        string spawnedObjectName = (location + " sound " + sound.name);
        GameObject spawnedObject = new GameObject(spawnedObjectName);
        Instantiate(spawnedObject, location, Quaternion.identity, this.transform);
        sound.audioSources[location] = spawnedObject.AddComponent<AudioSource>();
        Destroy(spawnedObject, sound.clip.length);

#if !UNITY_EDITOR
        UpdateAudioSource(sound, location);
#endif
        if (sound.hasCooldown)
        {
            soundTimerDictionary[GetSoundName(sound)] = -100;
        }
    }

    protected void UpdateAudioSource(Sound sound, AudioSource source)
    {
        source.clip = sound.clip;
        source.outputAudioMixerGroup = mixerGroup;
        source.volume = sound.volume;
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

    protected Sound FindSound(string name, Vector3 location)
    {
        foreach (SoundCollection soundCollection in sounds)
        {
            foreach (Sound sound in soundCollection.sounds)
            {
                if (sound.filename.Equals(name))
                {
                    if (!sound.audioSources.ContainsKey(location) || sound.audioSources[location] == null)
                    {
                        CreateAudioSource(sound, location);
                    }
#if UNITY_EDITOR
                    UpdateAudioSource(sound, sound.audioSources[location]);
#endif
                    return sound;
                }
            }
        }

        Debug.LogError("Sound " + name + " Not Found!");
        return null;
    }

    // Play sound with name, will be created if it doesn't exist
    public void Play(string name)
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
        sound.componentAudioSource.Play();
        sound.componentAudioSource.pitch = sound.pitch;
        sound.componentAudioSource.volume = sound.volume;
    }

    // Plays sound with name at location, creates a new one if it doesn't exist at absolute location. Sound temp object is deleted after being played
    public void Play(string name, Vector3 location)
    {
        Sound sound = FindSound(name, location);

        if (!CanPlaySound(sound))
        {
            return;
        }
        if (sound.pitchVarience > 0)
        {
            sound.audioSources[location].pitch += UnityEngine.Random.Range(-sound.pitchVarience, sound.pitchVarience);
        }
        if (sound.volumeVarience > 0)
        {
            sound.audioSources[location].volume += UnityEngine.Random.Range(-sound.volumeVarience, sound.volumeVarience);
        }
        sound.audioSources[location].Play();
        sound.audioSources[location].pitch = sound.pitch;
        sound.audioSources[location].volume = sound.volume;
    }

    public void PlayRandom(string name, int size)
    {
        Play(name.SetNumber(UnityEngine.Random.Range(1, size + 1)));
    }

    public void Stop(string name)
    {
        Sound sound = FindSound(name);
        soundTimerDictionary[GetSoundName(sound)] = 0;
        sound.componentAudioSource.Stop();
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