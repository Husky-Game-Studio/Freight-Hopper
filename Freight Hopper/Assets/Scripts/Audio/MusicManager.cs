using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : SoundManager
{
    private static MusicManager instance;
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

    private void Awake()
    {
        if (instance == null || currentSongName != lastSongName)
        {
            if (currentSongName != Songs.Menu)
            {
                DontDestroyOnLoad(this);
                instance = this;
                lastSongName = currentSongName;
            }
        }
        else
        {
            Destroy(this);
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

    public void SwitchSong(Songs songName)
    {
        Stop(currentSongName.ToString());
        Play(songName.ToString());
    }
}