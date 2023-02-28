using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    private int shadowDistance;
    private int fov;
    private float mouseSensitivity;
    private float soundEffectsVolume;
    private float musicVolume;

    public bool Vsync { get; private set; }
    public bool Antialiasing { get; private set; }
    public bool Continuous { get; private set; }
    public bool PlayerCollision { get; private set; }
    public int ShadowDistance => shadowDistance;
    public int FOV => fov;
    public float MouseSensitivity => mouseSensitivity;
    public float SoundEffectsVolume => soundEffectsVolume;
    public float MusicVolume => musicVolume;

    [SerializeField] private UniversalRenderPipelineAsset pipelineAsset;
    [SerializeField] private AudioMixer musicVolumeMixer;
    [SerializeField] private AudioMixer soundEffectsVolumeMixer;

    private enum SettingName
    {
        VSync=0,
        Antialiasing=1,
        ShadowDistance=2,
        Fov=3,
        MouseXSensitivity=2,
        MouseYSensitivity=4,
        SoundEffectsVolume=5,
        MusicVolume=6,
        Continuous=7,
        PlayerCollision=8
    }

    public void Start()
    {
        LoadSettings();
        SetSettings();
    }

    public void LoadSettings()
    {
        Vsync = Convert.ToBoolean(PlayerPrefs.GetInt(SettingName.VSync.ToString(), 1));
        Continuous = GetIsContinuousMode;
        PlayerCollision = GetIsPlayerCollisionEnabled;
        Antialiasing = Convert.ToBoolean(PlayerPrefs.GetInt(SettingName.Antialiasing.ToString(), 1));
        shadowDistance = PlayerPrefs.GetInt(SettingName.ShadowDistance.ToString(), 1000);
        fov = GetFOV;
        mouseSensitivity = GetMouseSensitivity;
        soundEffectsVolume = PlayerPrefs.GetFloat(SettingName.SoundEffectsVolume.ToString(), 0.5f);
        musicVolume = PlayerPrefs.GetFloat(SettingName.MusicVolume.ToString(), 0.5f);
    }

    public void SetSettings()
    {
        QualitySettings.vSyncCount = Vsync ? 1 : 0;
        pipelineAsset.msaaSampleCount = Antialiasing ? 4 : 1;
        pipelineAsset.shadowDistance = shadowDistance;
        UpdateMixerVolume(musicVolumeMixer, musicVolume);
        UpdateMixerVolume(soundEffectsVolumeMixer, soundEffectsVolume);
    }

    public static int GetFOV => PlayerPrefs.GetInt(SettingName.Fov.ToString(), 100);
    public static float GetMouseSensitivity => PlayerPrefs.GetFloat(SettingName.MouseXSensitivity.ToString(), 2);
    public static bool GetIsPlayerCollisionEnabled => Convert.ToBoolean(PlayerPrefs.GetInt(SettingName.PlayerCollision.ToString(), 0));
    public static bool GetIsContinuousMode => Convert.ToBoolean(PlayerPrefs.GetInt(SettingName.Continuous.ToString(), 0));


    public void SaveSettings()
    {
        PlayerPrefs.SetInt(SettingName.VSync.ToString(), Convert.ToInt32(Vsync));
        PlayerPrefs.SetInt(SettingName.Antialiasing.ToString(), Convert.ToInt32(Antialiasing));
        PlayerPrefs.SetInt(SettingName.Continuous.ToString(), Convert.ToInt32(Continuous));
        PlayerPrefs.SetInt(SettingName.PlayerCollision.ToString(), Convert.ToInt32(PlayerCollision));
        PlayerPrefs.SetInt(SettingName.ShadowDistance.ToString(), shadowDistance);
        PlayerPrefs.SetInt(SettingName.Fov.ToString(), fov);
        PlayerPrefs.SetFloat(SettingName.MouseXSensitivity.ToString(), mouseSensitivity);
        PlayerPrefs.SetFloat(SettingName.SoundEffectsVolume.ToString(), soundEffectsVolume);
        PlayerPrefs.SetFloat(SettingName.MusicVolume.ToString(), musicVolume);
    }

    private void UpdateMixerVolume(AudioMixer mixer, float vol)
    {
        if (Mathf.Abs(vol) < float.Epsilon)
        {
            mixer.SetFloat("volume", -100f);
            
        }
        else
        {
            mixer.SetFloat("volume", Mathf.Log10(vol) * 20);
        }
    }

    public void SetVsync(bool val) => Vsync = val;
    public void SetAntialiasing(bool val) =>Antialiasing = val;
    public void SetContinuous(bool val) => Continuous = val;
    public void SetRandom(bool val) => PlayerCollision = val;

    public void SetShadowDistance(float val) => shadowDistance = (int)val;
  

    public void SetFOV(float val) => fov = (int)val;

    public void SetMouseSens(float val) => mouseSensitivity = val;

    public void SetSoundEffectsVolume(float val)
    {
        soundEffectsVolume = val;
        UpdateMixerVolume(soundEffectsVolumeMixer, soundEffectsVolume);
    }

    public void SetMusicVolume(float val)
    {
        musicVolume = val;
        UpdateMixerVolume(musicVolumeMixer, musicVolume);
    }
}