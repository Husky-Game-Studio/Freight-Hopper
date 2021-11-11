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

    [SerializeField] private AudioMixerSnapshot normal;
    [SerializeField] private AudioMixerSnapshot paused;
    [SerializeField] private AudioMixerSnapshot alternate;

    public enum SnapshotMode
    { Normal, Paused, Alternate }

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
    [SerializeField] private SnapshotMode musicMode = SnapshotMode.Normal;
    [SerializeField] private bool playMusicOnAwake = true;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this);
            instance = this;
            if (currentSongName != Songs.Menu)
            {
                lastSongName = currentSongName;
            }
        }
        else
        {
            if (instance == this)
            {
                return;
            }
            if (currentSongName != lastSongName && currentSongName != Songs.Menu)
            {
                instance.SwitchSong(currentSongName);
                lastSongName = currentSongName;
            }
            instance.TransitionToSnapshot(musicMode);
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

    public void TransitionToSnapshot(SnapshotMode mode)
    {
        switch (mode)
        {
            case SnapshotMode.Normal:
                normal.TransitionTo(0.5f);
                break;

            case SnapshotMode.Paused:
                paused.TransitionTo(0.5f);
                break;

            case SnapshotMode.Alternate:
                alternate.TransitionTo(0.5f);
                break;
        }
    }

    public void SwitchSong(Songs songName)
    {
        Stop(currentSongName.ToString());
        Play(songName.ToString());
    }
}