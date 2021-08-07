using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : SoundManager
{
    private static MusicManager instance;
    public static MusicManager Instance => instance;
    private static Songs lastSongName;

    public enum Songs
    {
        Menu,
        Desert,
        Factory,
        CityDay,
        CityNight,
        Ice,
        Sky,
        Space,
        Escape,
        Unknown
    }

    [SerializeField] private Songs currentSongName;
    [SerializeField] private bool playMusicOnAwake = true;
    private bool subscribedToLevelLoading = false;
    private static bool changeMixer = false;

    private void Awake()
    {
        if (instance != null && instance.mixerGroup != this.mixerGroup)
        {
            instance.mixerGroup = this.mixerGroup;
            changeMixer = true;
        }
        if (instance == null || currentSongName != lastSongName)
        {
            if (currentSongName != Songs.Menu)
            {
                DontDestroyOnLoad(this);
                instance = this;
                lastSongName = currentSongName;
                SceneManager.sceneLoaded += LevelLoaded;
                subscribedToLevelLoading = true;
            }
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDisable()
    {
        if (subscribedToLevelLoading)
        {
            SceneManager.sceneLoaded -= LevelLoaded;
        }
    }

    private void Start()
    {
        if (playMusicOnAwake)
        {
            Play(currentSongName.ToString());
        }
    }

    private void Update()
    {
        if (lastSongName != currentSongName)
        {
            Destroy(this.gameObject);
        }
    }

    private void LevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (changeMixer && this == instance && mode == LoadSceneMode.Single && SceneManager.GetActiveScene() == scene)
        {
            changeMixer = false;
            instance.ChangeMixer(mixerGroup);
        }
    }

    public void ChangeMixer(AudioMixerGroup newMixer)
    {
        Sound sound = FindSound(currentSongName.ToString());
        sound.componentAudioSource.outputAudioMixerGroup = newMixer;
    }

    public void SwitchSong(Songs songName)
    {
        Stop(currentSongName.ToString());
        Play(songName.ToString());
    }
}