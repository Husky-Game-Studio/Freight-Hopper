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
        startTime = Time.time;
        textTimer = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        float actualTime = Time.time - startTime;

        string minutes = ((int)actualTime / 60).ToString();
        string seconds = (actualTime % 60).ToString("f3");

        textTimer.text = minutes + ":" + seconds;
    }
}