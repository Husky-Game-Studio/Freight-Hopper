using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private AudioMixerGroup pausedAudioMixer;
    private bool paused = false;
    private AudioMixerGroup lastAudioMixer;
    private static PauseMenu instance;
    public static PauseMenu Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        menu.SetActive(false);
        UserInput.Instance.UserInputMaster.Player.Pause.performed += Pause;
    }

    private void Pause(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (paused)
        {
            PauseMenuDisable();
        }
        else
        {
            PauseMenuEnable();
        }
    }

    private void OnDisable()
    {
        UserInput.Instance.UserInputMaster.Player.Pause.performed -= Pause;
    }

    public void PauseMenuEnable()
    {
        paused = true;
        PauseGame();
        menu.SetActive(true);
    }

    public void PauseMenuDisable()
    {
        paused = false;
        ContinueGame();
        menu.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (MusicManager.Instance != null)
        {
            lastAudioMixer = MusicManager.Instance.MixerGroup;
            MusicManager.Instance.ChangeMixer(pausedAudioMixer);
        }
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ChangeMixer(lastAudioMixer);
        }
    }

    public void ExitToMenu()
    {
        PauseMenuDisable();
        SceneLoader.LoadMenu();
    }
}