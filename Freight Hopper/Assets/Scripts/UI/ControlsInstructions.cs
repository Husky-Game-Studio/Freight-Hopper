using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsInstructions : MonoBehaviour
{
    [SerializeField] private Image tutorialImage;
    private const float fadeInTime = 0.8f;
    private TextMeshProUGUI levelNameText;

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

        if(tutorialImage.enabled) 
        {
            tutorialImage.color = new Color(tutorialImage.color.r, tutorialImage.color.g, tutorialImage.color.b, 0f);
            StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
        Color initialColor = tutorialImage.color;   
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 1f);

        float elapsedTime = 0f;

        while (elapsedTime < fadeInTime)
        {
            elapsedTime += Time.deltaTime;
            tutorialImage.color = Color.Lerp(initialColor, targetColor, elapsedTime / fadeInTime);
            yield return null;
        }
    }
}
