using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class LevelComplete : MonoBehaviour
{
    [SerializeField] private GameObject levelCompleteScreen;
    [SerializeField] private LevelTimer timer;
    [SerializeField] private Image medalImage;
    [SerializeField] private SpriteList medalImages;
    
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI nextMedalTimeText;
    
    [SerializeField] private TextMeshProUGUI timerValue;
    [SerializeField] private TextMeshProUGUI bestTimeValue;
    [SerializeField] private TextMeshProUGUI nextMedalTimeValue;

    private void Awake()
    {  
        levelCompleteScreen.SetActive(false);
    }


    private void OnEnable()
    {
        Goal.LevelComplete += EnableLevelComplete;
        UserInput.Instance.UserInputMaster.Player.Menu.performed += Menu;
        UserInput.Instance.UserInputMaster.Player.Next.performed += NextLevel;
        UserInput.Instance.UserInputMaster.Player.Restart.performed += RestartLevel;
    }
    private void OnDisable()
    {
        Goal.LevelComplete -= EnableLevelComplete;
        UserInput.Instance.UserInputMaster.Player.Menu.performed -= Menu;
        UserInput.Instance.UserInputMaster.Player.Next.performed -= NextLevel;
        UserInput.Instance.UserInputMaster.Player.Restart.performed -= RestartLevel;
    }
    private void EnableLevelComplete()
    {
        InGameStates.Instance.SwitchState(InGameStates.States.LevelComplete);
        timer.gameObject.SetActive(false);
        Player.Instance.modules.soundManager.StopAll();

        
        BestTime();
        PauseMenu.Instance.PauseGame();
        levelCompleteScreen.SetActive(true);
    }

    private void BestTime()
    {
        string levelName = LevelController.Instance.CurrentLevelName.CurrentLevel();
        
        levelNameText.text = "Level " + levelName.Split(' ')[1]; // this just happens to always get the last number
        
        
        LevelSaveData levelTimeData = LevelTimeSaveLoader.Load(levelName);
        float myTime = timer.GetTime();
        timerText.text = "Time:";
        timerValue.text = LevelTimer.GetTimeString(myTime);
        if (levelTimeData == null)
        {
            Debug.Log("no save data found, saving new time");
            
            levelTimeData = new LevelSaveData();
            bestTimeText.text = "Best Time:";
            bestTimeValue.text = LevelTimer.GetTimeString(myTime);
            levelTimeData.LevelName = levelName;
            levelTimeData.SetNewBestTime(myTime, true);
            
            EventBoat.NewBestTime(levelName, myTime);
        }
        else
        {
            levelTimeData.LevelName = levelName;
            var newBestTime = levelTimeData.SetNewBestTime(myTime);
            bestTimeText.text = "Best Time:";
            bestTimeValue.text = LevelTimer.GetTimeString(newBestTime.Time);
            if(newBestTime.IsNew){
                EventBoat.NewBestTime(levelName, myTime);
            }
        }
        EventBoat.AddLevelComplete(levelName);

        //////////////////// Medal Shit ////////////////////
        float bestTime = levelTimeData.BestTime;
        int index = 0;
        while (index < 4 && bestTime < LevelController.Instance.levelData.MedalTimes[index])
        {
            if (index == 0)
            {
                EventBoat.BronzeMedalUnlock(levelName);
            }
            else if (index == 1)
            {
                EventBoat.SilverMedalUnlock(levelName);
            }
            else if (index == 2)
            {
                EventBoat.GoldMedalUnlock(levelName);
            }
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
            nextMedalTimeValue.gameObject.SetActive(true);
            nextMedalTimeText.text = "Next Medal Time:";
            nextMedalTimeValue.text = LevelTimer.GetTimeString(LevelController.Instance.levelData.MedalTimes[levelTimeData.MedalIndex + 1]);
        }
        else
        {
            nextMedalTimeText.gameObject.SetActive(false);
            nextMedalTimeValue.gameObject.SetActive(false);
        }
    }

    public void RestartLevel(){
        if (!InGameStates.Instance.StateIs(InGameStates.States.LevelComplete))
        {
            return;
        }
        PauseMenu.Instance.ContinueGame();
        LevelController.Instance.Respawn();
    }
    public void NextLevel(){
        if (!InGameStates.Instance.StateIs(InGameStates.States.LevelComplete))
        {
            return;
        }
        PauseMenu.Instance.ContinueGame();
        LevelController.Instance.LoadNextLevel();
    }
    public void Menu(){
        if (!InGameStates.Instance.StateIs(InGameStates.States.LevelComplete))
        {
            return;
        }
        PauseMenu.Instance.ContinueGame();
        SceneLoader.LoadMenu();
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