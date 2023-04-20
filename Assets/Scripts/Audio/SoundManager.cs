using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System.Collections;
using UnityEngine.Scripting;

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

    public void Mute()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        foreach (var source in sources)
        {
            source.enabled = false;
        }
    }
    public void UnMute()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        foreach (var source in sources)
        {
            source.enabled = true;
        }
    }
    
    bool isVolumeModified = true;
    bool isMuted = false;
    public void ResetVolumePercent()
    {
        if (!isVolumeModified) return;
        isVolumeModified = false;
        isMuted = false;
        foreach (SoundCollection soundCollection in sounds)
        {
            foreach (Sound sound in soundCollection.sounds)
            {
                if (sound.componentAudioSource == null || sound.componentAudioSource.clip == null) continue;

                sound.componentAudioSource.volume = sound.volume;
            }
        }
    }
    // NOTE: Percent of percent of persons volume. So if there volume is 20% and you pass 10%, its 10% of 20%
    public void ModifyVolumePercent(float percent)
    {
        isVolumeModified = true;
        isMuted = false;
        foreach (SoundCollection soundCollection in sounds)
        {
            foreach (Sound sound in soundCollection.sounds)
            {
                if (sound.componentAudioSource == null || sound.componentAudioSource.clip == null) continue;

                sound.componentAudioSource.volume = sound.volume * percent;
            }
        }
    }

    public void MuteVolume()
    {
        if (isMuted) return;
        isMuted = true;
        isVolumeModified = true;
        foreach (SoundCollection soundCollection in sounds)
        {
            foreach (Sound sound in soundCollection.sounds)
            {
                if (sound.componentAudioSource == null || sound.componentAudioSource.clip == null) continue;

                sound.componentAudioSource.volume = 0;
            }
        }
    }
    
    public void Play(string name, bool playMultiple = false)
    {
        if (this.gameObject.activeSelf == false) return;
        StartCoroutine(PlayAsync(name, playMultiple));
    }
    // Play sound with name, will be created if it doesn't exist
    [Preserve]
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
        
        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.loop = sound.isLoop;
        source.priority = sound.priority;
        source.spatialBlend = sound.spatialBlend;
        source.dopplerLevel = sound.dopplerLevel;
        source.spread = 360;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = sound.minVolumeDistance;
        source.maxDistance = sound.maxVolumeDistance;

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

    public void SetMaxDistance(string songName, float maxDistance)
    {
        Sound sound = FindSound(songName);
        if (sound.componentAudioSource == null) return;

        sound.componentAudioSource.maxDistance = maxDistance;
    }
    
    private IEnumerator Fade(Sound sound, float duration, float finalVolume)
    {
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
            yield return new WaitForSecondsRealtime(seconds);
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
            if (sound.componentAudioSource != null)
            {
                sound.componentAudioSource.Stop();
            }
            AddressableAssets.ReleaseAllOfAsset(sound.assetReference);
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