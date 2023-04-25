using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using SteamTrain;
using System.Collections;
using UnityEngine.Serialization;

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
    [SerializeField] UILeaderboard leaderboard;
    [FormerlySerializedAs("speed")][SerializeField]  float colorChangeSpeed = 1f;
    public const float MAX_TIME = 24 * 60;
    public const float MIN_TIME = Mathf.PI/1.234f;

    bool isBestTime = false;
    
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
        SteamBus.OnTimeUploaded += DownloadLeaderboard;
    }
    private void OnDisable()
    {
        Goal.LevelComplete -= EnableLevelComplete;
        SteamBus.OnNewBestTime -= CallBestTimeSfx;
        SteamBus.OnTimeUploaded -= DownloadLeaderboard;
    }

    void DownloadLeaderboard(){
        leaderboard.LoadCurrentSceneLeaderboard();
    }
   
    private void CallBestTimeSfx(){
        Player.Instance.modules.soundManager.Play("BestTime");
        isBestTime = true;
    }

    void Update()
    {
        ConstantlyChangeBestTimeColorRainbow();
    }
    
    private void ConstantlyChangeBestTimeColorRainbow(){
        if(!isBestTime) return;

        float hue = (Time.unscaledTime * colorChangeSpeed) % 1f;
        Color bestColor = Color.HSVToRGB(hue, 1f, 1f);
        timerText.color = bestColor;
        timerValue.color = bestColor;
    }
    
    private void EnableLevelComplete()
    {
        InGameStates.Instance.SwitchState(InGameStates.States.LevelComplete);
        levelCompleteScreen.SetActive(true);
        medalImage.gameObject.SetActive(false);
        StartCoroutine(BestTime());
        if (Settings.GetIsContinuousMode){
            NextLevel();
            return;
        }

        timer.gameObject.SetActive(false);
        Player.Instance.modules.soundManager.StopAll();
        PauseMenu.Instance.PauseGame();
    }
    
    
    private IEnumerator BestTime()
    {
        string levelName = LevelController.Instance.CurrentLevelName.CurrentLevel();
        string versionedLevelName = LevelController.Instance.GetVersionedLevel;

        levelNameText.text = LevelController.Instance.LevelData.Title; // this just happens to always get the last number
        timerText.text = "Time:";
        float myTime = timer.GetTime();
        timerValue.text = LevelTimer.GetTimeString(myTime);

        LevelVersionData levelTimeData = SaveFile.Current.ReadLevelVersionData(versionedLevelName);
        LevelAchievementData levelAchievementData = SaveFile.Current.ReadLevelAchievementData(levelName);

        if (levelTimeData == null)
        {
            Debug.Log("no save data found, saving new time");
            levelTimeData = new LevelVersionData(myTime);
            SaveFile.Current.WriteLevelVersionData(versionedLevelName, levelTimeData);
        }
        if (levelAchievementData == null)
        {
            levelAchievementData = new LevelAchievementData
            {
                RobertoFound = false,
                MedalIndex = -1
            };
            SaveFile.Current.WriteLevelAchievementData(levelName, levelAchievementData);
        }
        else
        {
            DisplayMedal(levelAchievementData);
        }

        LeaderboardEntry result = new LeaderboardEntry();
        yield return SaveFile.GetMyUserTimeCached(versionedLevelName, result, myTime);
        
        //////////////////// Medal Shit ////////////////////
        float bestTime = MAX_TIME;
        if (result != null && result.timeSeconds != default){
            bestTime = result.timeSeconds;
        }
        int index = 0;
        while (index < 4 && bestTime < LevelController.Instance.LevelData.MedalTimes[index])
        {
            index++;
        }
        int mIndex = index - 1;
        if (levelAchievementData.MedalIndex < mIndex)
        {
            levelAchievementData.MedalIndex = mIndex;
            SaveFile.Current.WriteLevelAchievementData(levelName, levelAchievementData);
        }

        DisplayMedal(levelAchievementData);

        // Look, don't ruin the fun for others please
        LevelCompleteData.InvalidationReason reason = LevelCompleteData.InvalidationReason.None;

        if(myTime < MIN_TIME){
            reason = LevelCompleteData.InvalidationReason.ShortTime;
        } else if(Settings.GetIsPlayerCollisionEnabled) {
            reason = LevelCompleteData.InvalidationReason.CollisionEnabled;
        }

        if (reason != LevelCompleteData.InvalidationReason.None)
        {
            myTime = MAX_TIME;
            Debug.Log($"Submitting max time as runner doesn't meet qualifications");
        }
        LevelCompleteData data = new LevelCompleteData
        {
            World = 1,
            Level = versionedLevelName,
            Time = myTime,
            LevelInvalidationReason = reason
        };
        EventBoat.OnLevelComplete.Invoke(data);
    }

    void DisplayMedal(LevelAchievementData saveData){
        if (saveData.MedalIndex < 0)
        {
            medalImage.gameObject.SetActive(false);
        }
        else
        {
            medalImage.gameObject.SetActive(true);
            medalImage.sprite = medalImages.Sprites[saveData.MedalIndex];
        }

        if (saveData.MedalIndex < 2)
        {
            nextMedalTimeText.gameObject.SetActive(true);
            nextMedalTimeValue.gameObject.SetActive(true);
            breakpointImage.SetActive(true);
            nextMedalTimeText.text = "Medal Time:";
            nextMedalTimeValue.text = LevelTimer.GetTimeString(LevelController.Instance.LevelData.MedalTimes[saveData.MedalIndex + 1]);
        }
        else
        {
            nextMedalTimeText.gameObject.SetActive(false);
            nextMedalTimeValue.gameObject.SetActive(false);
            breakpointImage.SetActive(false);
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
        if(!Settings.GetIsContinuousMode){
            PauseMenu.Instance.ContinueGame();
        }
        
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