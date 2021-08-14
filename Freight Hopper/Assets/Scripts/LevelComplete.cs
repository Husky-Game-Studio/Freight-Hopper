using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelComplete : MonoBehaviour
{
    [SerializeField] private GameObject levelCompleteScreen;
    [SerializeField] private LevelTimer timer;
    [SerializeField] private TextMeshProUGUI timerText;

    private void Awake()
    {
        if (Goal.Instance == null)
        {
            Debug.Log("No Goal Found");
        }
        else
        {
            Goal.Instance.SetLevelCompleteScreen(EnableLevelComplete);
        }

        levelCompleteScreen.SetActive(false);
    }

    private void EnableLevelComplete()
    {
        if (Goal.Instance == null)
        {
            Debug.LogWarning("Goal was never found");
            return;
        }

        timerText.text = timer.GetTimeString();
        PauseMenu.Instance.PauseGame();
        levelCompleteScreen.SetActive(true);
    }

    public void RestartLevel()
    {
        PauseMenu.Instance.ContinueGame();
        LevelController.Instance.Respawn();
    }

    public void NextLevel()
    {
        PauseMenu.Instance.ContinueGame();
        LevelController.Instance.LoadNextLevel();
    }

    public void Menu()
    {
        PauseMenu.Instance.ContinueGame();
        SceneLoader.LoadMenu();
    }
}