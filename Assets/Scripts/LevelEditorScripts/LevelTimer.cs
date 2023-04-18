using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using TMPro;

// Source: https://www.youtube.com/watch?v=x-C95TuQtf0
public class LevelTimer : MonoBehaviour
{
    TextMeshProUGUI textTimer;
    DateTime startDateTime;
    Stopwatch stopwatch;
    void Start()
    {
        startDateTime = DateTime.UtcNow;
        stopwatch = Stopwatch.StartNew();
        textTimer = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        textTimer.text = GetTimeString(GetTime());
    }

    public float GetTime() //ur not kool
    {
        float utcTime = (float)(DateTime.UtcNow-startDateTime).TotalSeconds;
        float reportedTime = Mathf.Max(utcTime, stopwatch.ElapsedMilliseconds / 1000f);
        return reportedTime;
    }

    public static string GetTimeString(float time)
    {
        int minutes = (int)time / 60;
        float seconds = time % 60;

        StringBuilder sb = new StringBuilder();
        if (minutes > 0)
        {
            if (minutes < 10)
            {
                sb.Append("0");
            }
            sb.Append(minutes);
            sb.Append(":");
        }
        if (seconds < 10)
        {
            sb.Append("0");
        }
        sb.Append(seconds.ToString("f3"));
        return sb.ToString();
    }
}