using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelComplete : MonoBehaviour
{
    [SerializeField] private GameObject levelCompleteScreen;
    [SerializeField] private LevelTimer timer;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI levelNameText;

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
        BestTime();
        PauseMenu.Instance.PauseGame();
        levelCompleteScreen.SetActive(true);
    }

    private void BestTime()
    {
        string levelName = LevelController.Instance.CurrentLevelName.CurrentLevel();
        levelNameText.text = levelName;
        LevelTimeSaveData levelTimeData = LevelTimeSaveLoader.Load(levelName);
        if (levelTimeData == null)
        {
            bestTimeText.text = "Best Time: " + timer.GetTimeString();
            LevelTimeSaveData.AbilityTimes time = new LevelTimeSaveData.AbilityTimes(LevelController.Instance.levelData.activeAbilities, timer.GetTime());
            List<LevelTimeSaveData.AbilityTimes> times = new List<LevelTimeSaveData.AbilityTimes>
            {
                time
            };
            levelTimeData = new LevelTimeSaveData(times);
            LevelTimeSaveLoader.Save(levelName, levelTimeData);
            Debug.Log("no save data found, saving new best of " + timer.GetTime());
        }
        else
        {
            float bestTime = levelTimeData.SetNewBestTime(LevelController.Instance.levelData.activeAbilities, timer.GetTime());
            bestTimeText.text = "Best Time: " + timer.GetTimeString(bestTime);
            if (bestTime == timer.GetTime())
            {
                LevelTimeSaveLoader.Save(levelName, levelTimeData);
                Debug.Log("saving new best time of " + bestTime);
            }
        }
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