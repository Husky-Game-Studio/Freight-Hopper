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
    [SerializeField] private Timer minSongLengthTimer = new Timer(60);
    [SerializeField] private float musicChangeChance = 0.1f;

    public enum SnapshotMode
    { Normal, Paused, Alternate }

    public enum Songs
    {
        Menu,
        CityDay,
        CityNight,
        Desert,
        Escape,
        Factory,
        Ice,
        Sky,
        Space,
        Unknown
    }

    private Songs currentSong = Songs.Menu;
    [SerializeField] private SnapshotMode musicMode = SnapshotMode.Normal;
    private string lastScene = "";
    private void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
            if (SceneManager.GetActiveScene().name.Equals("MainMenu"))
            {
                Play(currentSong.ToString());
            }
            else
            {
                OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
            }
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

    void Update()
    {
        if (Player.Instance != null)
        {
            this.transform.position = Player.Instance.transform.position;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        if(mode != LoadSceneMode.Single)
        {
            return;
        }
        
        if (!scene.name.Equals(SceneManager.GetActiveScene().name)) 
        {
            return;
        }

        if(!SceneManager.GetActiveScene().name.Equals("MainMenu") && !minSongLengthTimer.TimerActive() && !lastScene.Equals(scene.name))
        {
            if (UnityEngine.Random.Range(0f, 1f) < musicChangeChance || instance.currentSong == Songs.Menu)
            {
                instance.SwitchSong(PickRandomSong());
                instance.TransitionToSnapshot(musicMode);
                minSongLengthTimer.ResetTimer();
            }
        }
        lastScene = scene.name;
    }

    private Songs PickRandomSong(){
        List<Songs> enums = Enum.GetValues(typeof(Songs)).Cast<Songs>().ToList();
        Songs random;
        do
        {
            random = enums[UnityEngine.Random.Range(1, enums.Count)];
        }
        while (instance.currentSong == random);
        
        return random;
    }

    private void FixedUpdate()
    {
        minSongLengthTimer.CountDown(Time.fixedUnscaledDeltaTime);
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