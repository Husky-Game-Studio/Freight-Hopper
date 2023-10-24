using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using SteamTrain;

public class SelectedLevelDataManager : MonoBehaviour
{
    [SerializeField] private Image medal;
    [SerializeField] private RawImage levelImage;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI worldtitle;
    [SerializeField] private TextMeshProUGUI bestTime;
    [SerializeField] private TextMeshProUGUI place;
    [SerializeField] private LevelListManager levelListManager;
    [SerializeField] private SpriteList medalSprites;
    private int lastIndex;

    public LevelData CurrentLevelData => levelListManager.CurrentWorld.Levels[LevelSelectLevelButton.currentID - 1];

    public IEnumerator UpdateUI()
    {
        LevelData currentData = this.CurrentLevelData;
        if(currentData == null)
        {
            place.enabled = false;
            yield break;
        }
        
        worldtitle.text = levelListManager.CurrentWorld.name;
        title.text = currentData.Title;
        
        LevelAchievementData levelSaveData = SaveFile.Current.ReadLevelAchievementData(currentData.name);
        if (levelSaveData != null)
        {
            if (levelSaveData.MedalIndex >= 0 && levelSaveData.MedalIndex < medalSprites.Sprites.Count)
            {
                medal.gameObject.SetActive(true);
                medal.sprite = medalSprites.Sprites[levelSaveData.MedalIndex];
            }
            else
            {
                medal.gameObject.SetActive(false);
            }
        }
        else
        {
            bestTime.enabled = false;
            place.enabled = false;
            medal.gameObject.SetActive(false);
        }
        levelImage.texture = currentData.Image;

        Button button = levelImage.GetComponent<Button>();
        if (button.onClick.GetPersistentEventCount() > 0)
        {
            button.onClick.RemoveAllListeners();
        }
        button.onClick.AddListener(delegate { SceneLoader.LoadLevel(currentData.name); });

        LevelName levelName = new LevelName(currentData.name);
        LeaderboardEntry result = new LeaderboardEntry();
        yield return SaveFile.GetMyUserTimeUncached(levelName.VersionedCurrentLevel(currentData), result);
        if (result.timeSeconds == 0)
        {
            bestTime.enabled = false;
            place.enabled = false;
        }
        else
        {
            bestTime.text = "Best: " + LevelTimer.GetTimeString(result.timeSeconds);
            bestTime.enabled = true;
            place.enabled = true;
            place.text = $"Place: {Ordinalifier(result.rank)}";
        }
    }
    string Ordinalifier(int rank)
    {
        int lastDigit = rank % 10;
        int last2Digits = rank % 100;
        if (last2Digits is 11 or 12 or 13) return $"{rank}th";
        switch (lastDigit)
        {
            case 1:
                return $"{rank}st";
            case 2:
                return $"{rank}nd";
            case 3:
                return $"{rank}rd";
            default:
                return $"{rank}th";
        }
    }
    public void OnEnable()
    {
        bestTime.enabled = false;
        place.enabled = false;
        StartCoroutine(UpdateUI());
    }
    private void Update()
    {
        if (LevelSelectLevelButton.currentID != lastIndex)
        {
            StartCoroutine(UpdateUI());
        }
        lastIndex = LevelSelectLevelButton.currentID;
    }
}