using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System.Collections;

// From comments from https://www.youtube.com/watch?v=QL29aTa7J5Q
// Probably heavily modified by the time anyone reads this
public class SoundManager : MonoBehaviour
{
    [SerializeField] protected AudioMixerGroup mixerGroup;
    public AudioMixerGroup MixerGroup => mixerGroup;
    [SerializeField] protected SoundCollection[] sounds;
    protected Dictionary<string, float> soundTimerDictionary = new Dictionary<string, float>();

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

    protected void CreateAudioSource(Sound sound, Vector3 location)
    {
        string spawnedObjectName = location + " sound " + sound.name;
        GameObject spawnedObject = new GameObject(spawnedObjectName);
        Instantiate(spawnedObject, location, Quaternion.identity, this.transform);
        sound.audioSources[location] = spawnedObject.AddComponent<AudioSource>();
        Destroy(spawnedObject, sound.clip.length);

#if !UNITY_EDITOR
        UpdateAudioSource(sound, sound.audioSources[location]);
#endif
        ResetTimer(sound);
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
        sound.componentAudioSource.playOnAwake = false;
        sound.componentAudioSource.volume = 0;
        StartCoroutine(Fade(sound.componentAudioSource, sound.fadeInTime, sound.volume));
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
        sound.audioSources[location].playOnAwake = false;
        sound.audioSources[location].volume = 0;
        StartCoroutine(Fade(sound.audioSources[location], sound.fadeInTime, sound.volume));
    }

    public void PlayRandom(string name, int size)
    {
        Play(name.SetNumber(UnityEngine.Random.Range(1, size + 1)));
    }

    public void Stop(string name)
    {
        Sound sound = FindSound(name);
        if (sound.componentAudioSource.isActiveAndEnabled)
        {
            StopAfterSeconds(sound.componentAudioSource, sound.fadeOutTime);
            StartCoroutine(Fade(sound.componentAudioSource, sound.fadeOutTime, 0));
            ResetTimer(sound);
        }
    }

    private IEnumerator StopAfterSeconds(AudioSource source, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        source.Stop();
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