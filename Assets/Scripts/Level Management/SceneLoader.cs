using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    /// Format for name: "World #"
    /// For example: 1 1
    public void LoadLevelButton(string levelName)
    {
        LoadLevel(levelName);
    }

    /// Format for name: "World #"
    /// For example: 1 1
    public static void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }

    public static void LoadMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        AudioListener.volume = 1;
        LevelSelectScreenOpener.SwitchSceneFlag(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}