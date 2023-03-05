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
    [SerializeField] private LevelListManager levelListManager;
    [SerializeField] private SpriteList medalSprites;
    private int lastIndex;

    public LevelData CurrentLevelData => levelListManager.CurrentWorld.Levels[LevelSelectLevelButton.currentID - 1];

    public IEnumerator UpdateUI()
    {
        LevelData currentData = this.CurrentLevelData;
        if(currentData == null)
        {
            yield break;
        }
        
        worldtitle.text = levelListManager.CurrentWorld.name;
        title.text = currentData.Title;

        LeaderboardEntry result = new LeaderboardEntry();
        yield return LeaderboardEventHandler.GetMyUserTime(currentData.name, result);
        LevelSaveData levelSaveData = LevelTimeSaveLoader.Load(currentData.name);
        if (levelSaveData != null)
        {
            if (result == null)
            {
                bestTime.text = "Best: None";
            }
            else
            {
                bestTime.text = "Best: " + LevelTimer.GetTimeString(result.timeSeconds);
            }
            if (levelSaveData.MedalIndex >= 0)
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
            bestTime.text = "Best: None";
            medal.gameObject.SetActive(false);
        }
        levelImage.texture = currentData.Image;

        Button button = levelImage.GetComponent<Button>();
        if (button.onClick.GetPersistentEventCount() > 0)
        {
            button.onClick.RemoveAllListeners();
        }
        button.onClick.AddListener(delegate { SceneLoader.LoadLevel(currentData.name); });
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