using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class MusicManager : SoundManager
{
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
        if (playMusicOnAwake)
        {
            Play(currentSongName.ToString());
        }
    }

    public void SwitchSong(Songs songName)
    {
        Stop(currentSongName.ToString());
        Play(songName.ToString());
    }
}