using System.Collections;
using System.Collections.Generic;
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
        textTimer.text = GetTimeString();
    }

    public float GetTime()
    {
        return Time.realtimeSinceStartup - startTime;
    }

    public string GetTimeString()
    {
        return GetTimeString(GetTime());
    }

    public static string GetTimeString(float time)
    {
        string minutes = ((int)time / 60).ToString();
        string seconds = (time % 60).ToString("f3");
        return minutes + ":" + seconds;
    }
}