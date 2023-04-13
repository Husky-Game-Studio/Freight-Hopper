using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControlsInstructions : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private CanvasGroup canvasGroup;
    private const float fadeInTime = 0.8f;

    Dictionary<string, string> levelInstructions = new Dictionary<string, string>() 
    { 
        { "1-1", "W - Move Forward\nMouse - Steer\nSpace - Jump\nShift - Ground Pound" }, 
        { "1-2", "Ground Pound on Slopes to Speed Up.\nShift - Ground Pound" }, 
        { "1-3", "Ground Pound on Slopes to Speed Up.\nShift - Ground Pound" }, 
        { "1-4", "Jumping + Ground Pound = Speed.\nShift - Ground Pound" }
    };

    private void Start()
    {
        canvasGroup.alpha = 0;
        
        string levelName = LevelController.Instance.CurrentLevelName.CurrentLevel();

        if (!levelInstructions.ContainsKey(levelName)){
            return;
        }

        LevelAchievementData levelTimeSaveData = SaveFile.Current.ReadLevelAchievementData(levelName);
        bool disableCanvas = levelTimeSaveData is { MedalIndex: > 0 };

        if (!disableCanvas) 
        {
            descriptionText.text = levelInstructions[levelName];
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeInTime)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = elapsedTime / fadeInTime;
            yield return null;
        }
    }
}
