using UnityEngine;
using UnityEngine.Serialization;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Toggle vsync;
    [SerializeField] private UnityEngine.UI.Toggle antialiasing;
    [SerializeField] private EchoSlider shadowDistance;
    [SerializeField] private EchoSlider fov;
    [FormerlySerializedAs("horizontalSensitivity")][SerializeField] private EchoSlider sensitivity;
    [SerializeField] private EchoSlider soundsVolume;
    [SerializeField] private EchoSlider musicVolume;

    [SerializeField] private Settings settings;

    public void BindUI()
    {
        vsync.isOn = settings.Vsync;
        antialiasing.isOn = settings.Antialiasing;
        shadowDistance.SetSliderValue(settings.ShadowDistance);
        fov.SetSliderValue(settings.FOV);
        sensitivity.SetSliderValue(settings.MouseSensitivity);
        soundsVolume.SetSliderValue(settings.SoundEffectsVolume);
        musicVolume.SetSliderValue(settings.MusicVolume);

        vsync.onValueChanged.AddListener(settings.SetVsync);
        antialiasing.onValueChanged.AddListener(settings.SetAntialiasing);
        shadowDistance.SetListener(settings.SetShadowDistance);
        fov.SetListener(settings.SetFOV);
        sensitivity.SetListener(settings.SetMouseX);
        soundsVolume.SetListener(settings.SetSoundEffectsVolume);
        musicVolume.SetListener(settings.SetMusicVolume);
    }
}