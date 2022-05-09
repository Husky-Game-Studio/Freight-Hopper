using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelComplete : MonoBehaviour
{
    [SerializeField] private GameObject levelCompleteScreen;
    [SerializeField] private LevelTimer timer;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private Image medalImage;
    [SerializeField] private SpriteList medalImages;
    [SerializeField] private TextMeshProUGUI nextMedalTimeText;

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
        float bestTime;
        LevelTimeSaveData levelTimeData = LevelTimeSaveLoader.Load(levelName);
        if (levelTimeData == null)
        {
            bestTimeText.text = "Best Time: " + timer.GetTimeString();
            float time = timer.GetTime();
            levelTimeData = new LevelTimeSaveData(time);
            LevelTimeSaveLoader.Save(levelName, levelTimeData);
            Debug.Log("no save data found, saving new best of " + time);
            bestTime = time;
        }
        else
        {
            float newBestTime = levelTimeData.SetNewBestTime(timer.GetTime());
            bestTimeText.text = "Best Time: " + LevelTimer.GetTimeString(newBestTime);
            if (newBestTime == timer.GetTime())
            {
                LevelTimeSaveLoader.Save(levelName, levelTimeData);
                Debug.Log("saving new best time of " + newBestTime);
            }
            bestTime = newBestTime;
        }



        int index = 0;
        while (index < 4 && bestTime < LevelController.Instance.levelData.MedalTimes[index])
        {
            index++;
        }
        if (levelTimeData.MedalIndex != index - 1)
        {
            levelTimeData.SetNewMedalIndex(index - 1);
            LevelTimeSaveLoader.Save(levelName, levelTimeData);
        }
        if (levelTimeData.MedalIndex < 0)
        {
            medalImage.gameObject.SetActive(false);
        }
        else
        {
            medalImage.gameObject.SetActive(true);
            medalImage.sprite = medalImages.Sprites[levelTimeData.MedalIndex];
        }
        if (levelTimeData.MedalIndex < 2)
        {
            nextMedalTimeText.gameObject.SetActive(true);
            nextMedalTimeText.text = "Next: " + LevelTimer.GetTimeString(LevelController.Instance.levelData.MedalTimes[levelTimeData.MedalIndex + 1]);
        }
        else
        {
            nextMedalTimeText.gameObject.SetActive(false);
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