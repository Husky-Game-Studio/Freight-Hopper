using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsInstructions : MonoBehaviour
{
    [SerializeField] private Image tutorialImage;
    [SerializeField] private TextMeshProUGUI tutorialText;
    private const float fadeInTime = 0.8f;

    private void Start()
    {
        string levelName = LevelController.Instance.CurrentLevelName.CurrentLevel();
        LevelTimeSaveData levelTimeSaveData = LevelTimeSaveLoader.Load(levelName);
        if (levelTimeSaveData == null)
        {
            tutorialImage.enabled = true;
            tutorialText.enabled = true;
        }
        else
        {
            if (levelTimeSaveData.MedalIndex > 0)
            {
                tutorialImage.enabled = false;
                tutorialText.enabled = false;
            }
            else
            {
                tutorialImage.enabled = true;
                tutorialText.enabled = true;
            }
        }

        if(tutorialImage.enabled) 
        {
            tutorialImage.color = new Color(tutorialImage.color.r, tutorialImage.color.g, tutorialImage.color.b, 0f);
            tutorialImage.color = new Color(tutorialText.color.r, tutorialText.color.g, tutorialText.color.b, 0f);
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        Color initialColor = tutorialImage.color;
        Color initialColorText = tutorialText.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 1f);
        Color targetColorText = new Color(initialColorText.r, initialColorText.g, initialColorText.b, 1f);

        float elapsedTime = 0f;

        while (elapsedTime < fadeInTime)
        {
            elapsedTime += Time.deltaTime;
            tutorialImage.color = Color.Lerp(initialColor, targetColor, elapsedTime / fadeInTime);
            tutorialText.color = Color.Lerp(initialColorText, targetColorText, elapsedTime / fadeInTime);
            yield return null;
        }
    }
}
