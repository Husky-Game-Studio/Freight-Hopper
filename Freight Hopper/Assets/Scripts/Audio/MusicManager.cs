using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : SoundManager
{
    private static MusicManager instance;
    public static MusicManager Instance => instance;
    private static Songs lastSongName = Songs.Menu;

    [SerializeField] private AudioMixerSnapshot normal;
    [SerializeField] private AudioMixerSnapshot paused;
    [SerializeField] private AudioMixerSnapshot alternate;

    public enum SnapshotMode
    { Normal, Paused, Alternate }

    public enum Songs
    {
        Menu,
        Desert,
        CityDay,
        CityNight,
        Ice,
        Factory,
        Sky,
        Space,
        Escape,
        Unknown
    }

    [SerializeField] private Songs currentSongName = Songs.Menu;
    [SerializeField] private SnapshotMode musicMode = SnapshotMode.Normal;

    private void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
            Play(currentSongName.ToString());
        }
        else
        {
            if(instance.gameObject == this.gameObject)
            {
                return;
            }
            instance.TransitionToSnapshot(musicMode);
            if (currentSongName != lastSongName && currentSongName != Songs.Menu)
            {
                lastSongName = currentSongName;
                instance.SwitchSong(currentSongName);
                
            }
            Destroy(this.gameObject);
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
        currentSongName = songName;
    }
}