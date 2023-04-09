using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
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
        if (InGameStates.Instance.StateIs(InGameStates.States.Paused))
        {
            PauseMenuDisable();
        }
        else if(InGameStates.Instance.StateIs(InGameStates.States.Playing))
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
        InGameStates.Instance.SwitchState(InGameStates.States.Paused);
        PauseGame();
        menu.SetActive(true);
    }

    public void PauseMenuDisable()
    {
        InGameStates.Instance.SwitchState(InGameStates.States.Playing);
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
            MusicManager.Instance.TransitionToSnapshot(MusicManager.SnapshotMode.Paused);
        }
        Player.Instance.modules.soundManager.Mute();
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.TransitionToSnapshot(MusicManager.SnapshotMode.Normal);
        }
        Player.Instance.modules.soundManager.UnMute();
    }

    public void ExitToMenu()
    {
        PauseMenuDisable();
        SceneLoader.LoadMenu();
    }
}