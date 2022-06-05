using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsInstructions : MonoBehaviour
{
    [SerializeField] private Image tutorialImage;
    private TextMeshProUGUI levelNameText;

    // Start is called before the first frame update
    void Start()
    {
        string levelName = LevelController.Instance.CurrentLevelName.CurrentLevel();
        LevelTimeSaveData levelTimeSaveData = LevelTimeSaveLoader.Load(levelName);
        if (levelTimeSaveData == null)
        {
            tutorialImage.enabled = true;
        }
        else
        {
            if (levelTimeSaveData.MedalIndex > 0)
            {
                tutorialImage.enabled = false;
            }
            else
            {
                tutorialImage.enabled = true;
            }
        }

        
    }
}
