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

    private void Awake()
    {
        menu.SetActive(false);
        UserInput.Instance.UserInputMaster.Player.Pause.performed += Pause;
    }

    private void Pause(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (paused)
        {
            ContinueGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void OnDisable()
    {
        UserInput.Instance.UserInputMaster.Player.Pause.performed -= Pause;
    }

    public void PauseGame()
    {
        paused = true;
        Time.timeScale = 0;
        menu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        lastAudioMixer = MusicManager.Instance.MixerGroup;
        MusicManager.Instance.ChangeMixer(pausedAudioMixer);
    }

    public void ContinueGame()
    {
        paused = false;
        Time.timeScale = 1;
        menu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        MusicManager.Instance.ChangeMixer(lastAudioMixer);
    }

    public void ExitToMenu()
    {
        ContinueGame();
        SceneLoader.LoadMenu();
    }
}