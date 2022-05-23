using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    private bool vsync;
    private bool antialiasing;
    private int shadowDistance;
    private int fov;
    private Vector2 mouseSensitivity;
    private float soundEffectsVolume;
    private float musicVolume;

    public bool Vsync => vsync;
    public bool Antialiasing => antialiasing;
    public int ShadowDistance => shadowDistance;
    public int FOV => fov;
    public Vector2 MouseSensitivity => mouseSensitivity;
    public float SoundEffectsVolume => soundEffectsVolume;
    public float MusicVolume => musicVolume;

    [SerializeField] private UniversalRenderPipelineAsset pipelineAsset;
    [SerializeField] private AudioMixer musicVolumeMixer;
    [SerializeField] private AudioMixer soundEffectsVolumeMixer;

    enum SettingName
    {
        VSync,
        Antialiasing,
        ShadowDistance,
        Fov,
        MouseXSensitivity,
        MouseYSensitivity,
        SoundEffectsVolume,
        MusicVolume
    }

    private void Awake()
    {
        LoadSettings();
        SetSettings();
    }

    public void LoadSettings()
    {
        vsync = Convert.ToBoolean(PlayerPrefs.GetInt(SettingName.VSync.ToString(), 1));
        antialiasing = Convert.ToBoolean(PlayerPrefs.GetInt(SettingName.Antialiasing.ToString(), 1));
        shadowDistance = PlayerPrefs.GetInt(SettingName.ShadowDistance.ToString(), 1000);
        fov = PlayerPrefs.GetInt(SettingName.Fov.ToString(), 100);
        mouseSensitivity.x = PlayerPrefs.GetFloat(SettingName.MouseXSensitivity.ToString(), 14);
        mouseSensitivity.y = PlayerPrefs.GetFloat(SettingName.MouseYSensitivity.ToString(), 10);
        soundEffectsVolume = PlayerPrefs.GetFloat(SettingName.SoundEffectsVolume.ToString(), 0.5f);
        musicVolume = PlayerPrefs.GetFloat(SettingName.MusicVolume.ToString(), 0.5f);
    }

    public void SetSettings()
    {
        QualitySettings.vSyncCount = vsync ? 1 : 0;
        pipelineAsset.msaaSampleCount = antialiasing ? 4 : 0;
        pipelineAsset.shadowDistance = shadowDistance;
        UpdateMixerVolume(musicVolumeMixer, musicVolume);
        UpdateMixerVolume(soundEffectsVolumeMixer, soundEffectsVolume);
    }

    public static int GetFOV()
    {
        return PlayerPrefs.GetInt(SettingName.Fov.ToString(), 100);
    }
    public static Vector2 GetMouseSensitivity()
    {
        Vector2 mouseSens;
        mouseSens.x = PlayerPrefs.GetFloat(SettingName.MouseXSensitivity.ToString(), 14);
        mouseSens.y = PlayerPrefs.GetFloat(SettingName.MouseYSensitivity.ToString(), 10);
        return mouseSens;
    }
    public void SaveSettings()
    {
        PlayerPrefs.SetInt(SettingName.VSync.ToString(), Convert.ToInt32(vsync));
        PlayerPrefs.SetInt(SettingName.Antialiasing.ToString(), Convert.ToInt32(antialiasing));
        PlayerPrefs.SetInt(SettingName.ShadowDistance.ToString(), shadowDistance);
        PlayerPrefs.SetInt(SettingName.Fov.ToString(), fov);
        PlayerPrefs.SetFloat(SettingName.MouseXSensitivity.ToString(), mouseSensitivity.x);
        PlayerPrefs.SetFloat(SettingName.MouseYSensitivity.ToString(), mouseSensitivity.y);
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

    public void SetVsync(bool val)
    {
        vsync = val;
    }

    public void SetAntialiasing(bool val)
    {
        antialiasing = val;
    }

    public void SetShadowDistance(float val)
    {
        shadowDistance = (int)val;
    }

    public void SetFOV(float val)
    {
        fov = (int)val;
    }

    public void SetMouseX(float val)
    {
        mouseSensitivity.x = val;
    }

    public void SetMouseY(float val)
    {
        mouseSensitivity.y = val;
    }

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