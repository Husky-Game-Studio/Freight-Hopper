using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Toggle vsync;
    [SerializeField] private UnityEngine.UI.Toggle antialiasing;
    [SerializeField] private EchoSlider shadowDistance;
    [SerializeField] private EchoSlider fov;
    [SerializeField] private EchoSlider horizontalSensitivity;
    [SerializeField] private EchoSlider verticalSensitivity;
    [SerializeField] private EchoSlider soundsVolume;
    [SerializeField] private EchoSlider musicVolume;

    [SerializeField] private Settings settings;

    public void BindUI()
    {
        vsync.isOn = settings.Vsync;
        antialiasing.isOn = settings.Antialiasing;
        shadowDistance.SetSliderValue(settings.ShadowDistance);
        fov.SetSliderValue(settings.FOV);
        horizontalSensitivity.SetSliderValue(settings.MouseSensitivity.x);
        verticalSensitivity.SetSliderValue(settings.MouseSensitivity.y);
        soundsVolume.SetSliderValue(settings.SoundEffectsVolume);
        musicVolume.SetSliderValue(settings.MusicVolume);

        vsync.onValueChanged.AddListener(settings.SetVsync);
        antialiasing.onValueChanged.AddListener(settings.SetAntialiasing);
        shadowDistance.SetListener(settings.SetShadowDistance);
        fov.SetListener(settings.SetFOV);
        horizontalSensitivity.SetListener(settings.SetMouseX);
        verticalSensitivity.SetListener(settings.SetMouseY);
        soundsVolume.SetListener(settings.SetSoundEffectsVolume);
        musicVolume.SetListener(settings.SetMusicVolume);
    }
}