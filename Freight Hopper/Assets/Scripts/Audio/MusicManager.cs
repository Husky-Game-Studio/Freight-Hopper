using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class MusicManager : SoundManager
{
    private static MusicManager instance;
    public static MusicManager Instance => instance;

    [SerializeField] private AudioMixerSnapshot normal;
    [SerializeField] private AudioMixerSnapshot paused;
    [SerializeField] private AudioMixerSnapshot alternate;

    [SerializeField] private float musicChangeChance = 0.1f;

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
    Songs currentSong = Songs.Menu;
    [SerializeField] private SnapshotMode musicMode = SnapshotMode.Normal;
    private Scene lastScene;
    private void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
            Play(currentSong.ToString());
        }
        else
        {
            if(instance.gameObject == this.gameObject)
            {
                return;
            }
            
            
            Destroy(this.gameObject);
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {   
        if(mode != LoadSceneMode.Single){
            return;
        }
        if(!SceneManager.GetActiveScene().name.Equals("MainMenu") && mode == LoadSceneMode.Single && lastScene != scene)
        {
            if (UnityEngine.Random.Range(0f, 1f) < musicChangeChance || instance.currentSong == Songs.Menu)
            {
                instance.SwitchSong(PickRandomSong());
                instance.TransitionToSnapshot(musicMode);
            }
        }
        lastScene = scene;
    }
    Songs PickRandomSong(){
        List<Songs> enums = Enum.GetValues(typeof(Songs)).Cast<Songs>().ToList();
        System.Random random = new System.Random();
        return enums[random.Next(1, enums.Count)];
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
        Stop(currentSong.ToString());
        Play(songName.ToString());
        currentSong = songName;
    }
}