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

    private void Start()
    {
        LoadSettings();
        SetSettings();
    }

    public void LoadSettings()
    {
        vsync = Convert.ToBoolean(PlayerPrefs.GetInt("Vsync", 1));
        antialiasing = Convert.ToBoolean(PlayerPrefs.GetInt("Antialiasing", 1));
        shadowDistance = PlayerPrefs.GetInt("ShadowDistance", 1000);
        fov = PlayerPrefs.GetInt("Fov", 100);
        mouseSensitivity.x = PlayerPrefs.GetFloat("MouseXSensitivity", 14);
        mouseSensitivity.y = PlayerPrefs.GetFloat("MouseYSensitivity", 10);
        soundEffectsVolume = PlayerPrefs.GetFloat("SoundEffectsVolume", 0.5f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
    }

    public void SetSettings()
    {
        QualitySettings.vSyncCount = vsync ? 1 : 0;
        pipelineAsset.msaaSampleCount = antialiasing ? 4 : 0;
        pipelineAsset.shadowDistance = shadowDistance;
        FirstPersonCamera.fov = fov;
        FirstPersonCamera.mouseSensitivity = mouseSensitivity;
        UpdateMixerVolume(musicVolumeMixer, musicVolume);
        UpdateMixerVolume(soundEffectsVolumeMixer, soundEffectsVolume);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("Vsync", Convert.ToInt32(vsync));
        PlayerPrefs.SetInt("Antialiasing", Convert.ToInt32(antialiasing));
        PlayerPrefs.SetInt("ShadowDistance", shadowDistance);
        PlayerPrefs.SetInt("Fov", fov);
        PlayerPrefs.SetFloat("MouseXSensitivity", mouseSensitivity.x);
        PlayerPrefs.SetFloat("MouseYSensitivity", mouseSensitivity.y);
        PlayerPrefs.SetFloat("SoundEffectsVolume", soundEffectsVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }

    private void UpdateMixerVolume(AudioMixer mixer, float vol)
    {
        if (Mathf.Abs(vol) < float.Epsilon)
        {
            mixer.SetFloat("volume", -1000);
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