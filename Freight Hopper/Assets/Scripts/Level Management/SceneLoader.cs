using System.Collections;
using System.Collections.Generic;
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

    // Loads default scene as an additive scene
    public static void LoadPlayerScene()
    {
        SceneManager.LoadScene("DefaultScene", LoadSceneMode.Additive);
    }

    public static void LoadMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void LoadEditor()
    {
        SceneManager.LoadScene("LevelEditorBeginning");
    }
}