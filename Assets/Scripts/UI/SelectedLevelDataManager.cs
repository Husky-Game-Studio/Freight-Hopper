using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public void UpdateUI()
    {
        LevelData currentData = this.CurrentLevelData;
        if(currentData == null)
        {
            return;
        }

        worldtitle.text = levelListManager.CurrentWorld.name;
        title.text = currentData.Title;
        LevelSaveData levelSaveData = LevelTimeSaveLoader.Load(currentData.name);
        if (levelSaveData != null)
        {
            if (float.IsInfinity(levelSaveData.BestTime))
            {
                bestTime.text = "Best: None";
            }
            else
            {
                bestTime.text = "Best: " + LevelTimer.GetTimeString(levelSaveData.BestTime);
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
            UpdateUI();
        }
        lastIndex = LevelSelectLevelButton.currentID;
    }
}