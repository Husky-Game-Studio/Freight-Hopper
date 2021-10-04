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

    private void Update()
    {
        if (LevelSelectLevelButton.currentID != lastIndex)
        {
            LevelData currentData = currentWorld.Levels[LevelSelectLevelButton.currentID - 1];
            worldtitle.text = currentWorld.name;
            title.text = currentData.Title;
            LevelTimeSaveData levelSaveData = LevelTimeSaveLoader.Load(currentData.name);
            if (levelSaveData != null)
            {
                bestTime.text = "Best: " + LevelTimer.GetTimeString(levelSaveData.GetTime(currentData.ActiveAbilities).Item1);
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
                bestTime.text = "No Best Time";
                medal.gameObject.SetActive(false);
            }
            levelImage.texture = currentData.Image;
        }
        lastIndex = LevelSelectLevelButton.currentID;
    }
}