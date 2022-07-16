using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

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

    private bool levelComplete;

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
    private void OnEnable()
    {
        UserInput.Instance.UserInputMaster.Player.Menu.performed += Menu;
        UserInput.Instance.UserInputMaster.Player.Next.performed += NextLevel;
        UserInput.Instance.UserInputMaster.Player.Restart.performed += RestartLevel;
    }
    private void OnDisable()
    {
        UserInput.Instance.UserInputMaster.Player.Menu.performed -= Menu;
        UserInput.Instance.UserInputMaster.Player.Next.performed -= NextLevel;
        UserInput.Instance.UserInputMaster.Player.Restart.performed -= RestartLevel;
    }
    private void EnableLevelComplete()
    {
        if (Goal.Instance == null)
        {
            Debug.LogWarning("Goal was never found");
            return;
        }

        
        timer.gameObject.SetActive(false);
        BestTime();
        PauseMenu.Instance.PauseGame(true);
        levelCompleteScreen.SetActive(true);
        levelComplete = true;
    }

    private void BestTime()
    {
        string levelName = LevelController.Instance.CurrentLevelName.CurrentLevel();
        levelNameText.text = "Level: " + levelName.Split(' ')[1]; // this just happens to always get the last number
        
        
        LevelTimeSaveData levelTimeData = LevelTimeSaveLoader.Load(levelName);
        float myTime = timer.GetTime();
        timerText.text = "Time: " + LevelTimer.GetTimeString(myTime);
        if (levelTimeData == null)
        {
            Debug.Log("no save data found, saving new time");
            
            levelTimeData = new LevelTimeSaveData(myTime, levelName);
            bestTimeText.text = "Best Time: " + LevelTimer.GetTimeString(myTime);
        }
        else
        {
            float newBestTime = levelTimeData.SetNewBestTime(myTime);
            bestTimeText.text = "Best Time: " + LevelTimer.GetTimeString(newBestTime);
        }

        SetMedals(levelTimeData);
    }
    void SetMedals(LevelTimeSaveData levelTimeData)
    {
        float bestTime = levelTimeData.BestTime;
        int index = 0;
        while (index < 4 && bestTime < LevelController.Instance.levelData.MedalTimes[index])
        {
            index++;
        }
        
        if (levelTimeData.MedalIndex != index - 1)
        {
            levelTimeData.SetNewMedalIndex(index - 1);
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
            nextMedalTimeText.text = "Next Medal Time: " + LevelTimer.GetTimeString(LevelController.Instance.levelData.MedalTimes[levelTimeData.MedalIndex + 1]);
        }
        else
        {
            nextMedalTimeText.gameObject.SetActive(false);
        }
    }

    public void RestartLevel(){
        if (!levelComplete)
        {
            return;
        }
        PauseMenu.Instance.ContinueGame();
        LevelController.Instance.Respawn();
        levelComplete = false;
    }
    public void NextLevel(){
        if (!levelComplete)
        {
            return;
        }
        PauseMenu.Instance.ContinueGame();
        LevelController.Instance.LoadNextLevel();
        levelComplete = false;
    }
    public void Menu(){
        if (!levelComplete)
        {
            return;
        }
        PauseMenu.Instance.ContinueGame();
        SceneLoader.LoadMenu();
        levelComplete = false;
    }
    public void RestartLevel(InputAction.CallbackContext context)
    {
        RestartLevel();
    }

    public void NextLevel(InputAction.CallbackContext context)
    {
        NextLevel();
    }

    public void Menu(InputAction.CallbackContext context)
    {
        Menu();
    }
}