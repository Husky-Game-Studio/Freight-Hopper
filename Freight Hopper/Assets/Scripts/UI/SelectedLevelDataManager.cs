using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private WorldMetaData currentWorld;
    [SerializeField] private SpriteList medalSprites;
    private int lastIndex = 0;

    public LevelData CurrentLevelData => currentWorld.Levels[LevelSelectLevelButton.currentID - 1];

    public void UpdateUI()
    {
        LevelData currentData = this.CurrentLevelData;
        //Debug.Log(LevelSelectLevelButton.currentID - 1);
        worldtitle.text = currentWorld.name;
        title.text = currentData.Title;
        LevelTimeSaveData levelSaveData = LevelTimeSaveLoader.Load(currentData.name);
        if (levelSaveData != null)
        {
            if (levelSaveData.GetTime(currentData.ActiveAbilities).Item2 == -1)
            {
                bestTime.text = "Best: None";
            }
            else
            {
                bestTime.text = "Best: " + LevelTimer.GetTimeString(levelSaveData.GetTime(currentData.ActiveAbilities).Item1);
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