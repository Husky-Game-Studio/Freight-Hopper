using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using SteamTrain;

public class LevelComplete : MonoBehaviour
{
    [SerializeField] private GameObject levelCompleteScreen;
    [SerializeField] private LevelTimer timer;
    [SerializeField] private Image medalImage;
    [SerializeField] private SpriteList medalImages;
    
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI nextMedalTimeText;
    [SerializeField] private GameObject breakpointImage;
    
    [SerializeField] private TextMeshProUGUI timerValue;
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
        SteamBus.OnNewBestTime += CallBestTimeSfx;
    }
    private void OnDisable()
    {
        Goal.LevelComplete -= EnableLevelComplete;
        UserInput.Instance.UserInputMaster.Player.Menu.performed -= Menu;
        UserInput.Instance.UserInputMaster.Player.Next.performed -= NextLevel;
        UserInput.Instance.UserInputMaster.Player.Restart.performed -= RestartLevel;
        SteamBus.OnNewBestTime -= CallBestTimeSfx;
    }
    private void CallBestTimeSfx(){
        Debug.Log("GREAT JOB");
    }
    private void EnableLevelComplete()
    {
        InGameStates.Instance.SwitchState(InGameStates.States.LevelComplete);
        BestTime();
        if (Settings.GetIsContinuousMode){
            NextLevel();
            return;
        }

        timer.gameObject.SetActive(false);
        Player.Instance.modules.soundManager.StopAll();
        PauseMenu.Instance.PauseGame();
        levelCompleteScreen.SetActive(true);
    }

    private void BestTime()
    {
        string levelName = LevelController.Instance.CurrentLevelName.CurrentLevel();
        

        levelNameText.text = LevelController.Instance.worldListMetaData.GetWorld("Desert").GetLevel(levelName).Title; // this just happens to always get the last number
        
        LevelSaveData levelTimeData = LevelTimeSaveLoader.Load(levelName);
        float myTime = timer.GetTime();
        timerText.text = "Time:";
        timerValue.text = LevelTimer.GetTimeString(myTime);
        if (levelTimeData == null)
        {
            Debug.Log("no save data found, saving new time");
            levelTimeData = new LevelSaveData
            {
                LevelName = levelName
            };
            levelTimeData.SetNewBestTime(myTime, true);
        }
        else
        {
            levelTimeData.LevelName = levelName;
            levelTimeData.SetNewBestTime(myTime);
        }

        //////////////////// Medal Shit ////////////////////
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
            nextMedalTimeValue.gameObject.SetActive(true);
            breakpointImage.SetActive(true);
            nextMedalTimeText.text = "Medal Time:";
            nextMedalTimeValue.text = LevelTimer.GetTimeString(LevelController.Instance.levelData.MedalTimes[levelTimeData.MedalIndex + 1]);
        }
        else
        {
            nextMedalTimeText.gameObject.SetActive(false);
            nextMedalTimeValue.gameObject.SetActive(false);
            breakpointImage.SetActive(false);
        }

        LevelCompleteData data = new LevelCompleteData
        {
            World = 1,
            Level = levelName,
            Time = myTime
        };
        EventBoat.OnLevelComplete.Invoke(data);
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
        if(!Settings.GetIsContinuousMode){
            PauseMenu.Instance.ContinueGame();
        }
        
        if(Settings.GetIsRandomMode){
            LevelController.Instance.LoadRandomLevel();
        }else{
            LevelController.Instance.LoadNextLevel();
        }
        
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