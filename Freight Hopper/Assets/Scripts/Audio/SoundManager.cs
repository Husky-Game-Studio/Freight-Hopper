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
        sound.source = this.gameObject.AddComponent<AudioSource>();
        sound.source.clip = sound.clip;
        sound.source.outputAudioMixerGroup = mixerGroup;
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;
        sound.source.loop = sound.isLoop;

        if (sound.hasCooldown)
        {
            soundTimerDictionary[sound.filename] = 0f;
        }
    }

    protected Sound FindSound(string name)
    {
        foreach (SoundCollection soundCollection in sounds)
        {
            foreach (Sound sound in soundCollection.sounds)
            {
                if (sound.filename.Equals(name))
                {
                    if (sound.source == null)
                    {
                        CreateAudioSource(sound);
                    }
                    return sound;
                }
            }
        }

        Debug.LogError("Sound " + name + " Not Found!");
        return null;
    }

    public void Play(string name)
    {
        Sound sound = FindSound(name);

        if (!CanPlaySound(sound))
        {
            return;
        }
        if (sound.pitchVarience > 0)
        {
            sound.source.pitch += UnityEngine.Random.Range(-sound.pitchVarience, sound.pitchVarience);
        }
        if (sound.volumeVarience > 0)
        {
            sound.source.volume += UnityEngine.Random.Range(-sound.volumeVarience, sound.volumeVarience);
        }
        sound.source.Play();
        sound.source.pitch = sound.pitch;
        sound.source.volume = sound.volume;
    }

    public void Stop(string name)
    {
        Sound sound = FindSound(name);

        sound.source.Stop();
    }

    protected bool CanPlaySound(Sound sound)
    {
        if (soundTimerDictionary.ContainsKey(sound.filename))
        {
            float lastTimePlayed = soundTimerDictionary[sound.filename];

            if (lastTimePlayed + sound.clip.length < Time.time)
            {
                soundTimerDictionary[sound.filename] = Time.time;
                return true;
            }

            return false;
        }

        return true;
    }
}