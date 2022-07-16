using UnityEngine;
using TMPro;

// Source: https://www.youtube.com/watch?v=x-C95TuQtf0
public class LevelTimer : MonoBehaviour
{
    private TextMeshProUGUI textTimer;
    private float startTime;

    private void Start()
    {
        startTime = Time.realtimeSinceStartup;
        textTimer = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        textTimer.text = GetTimeString(GetTime());
    }

    public float GetTime()
    {
        return Time.realtimeSinceStartup - startTime;
    }

    public static string GetTimeString(float time)
    {
        string minutes = ((int)time / 60).ToString();
        string seconds = (time % 60).ToString("f3");
        if (time % 60 < 10)
        {
            seconds = "0" + seconds;
        }
        return minutes + ":" + seconds;
    }
}