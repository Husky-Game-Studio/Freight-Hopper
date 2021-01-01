using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    /// <summary>
    /// Format for name: "World #"
    /// For example: 1 1
    /// </summary>
    /// <param name="worldName"></param>
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);

        SceneManager.LoadScene("DefaultScene", LoadSceneMode.Additive);
    }

    public static void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}