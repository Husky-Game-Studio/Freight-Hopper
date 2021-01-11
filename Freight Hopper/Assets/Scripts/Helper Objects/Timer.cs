using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Timer
{
    public float duration;
    public float current;

    /// <summary>
    /// Initailizes duration and sets current to 0
    /// </summary>
    /// <param name="duration">duration timer lasts</param>
    public Timer(float duration)
    {
        this.duration = duration;
        current = 0f;
    }

    /// <summary>
    /// Sets current to duration
    /// </summary>
    public void ResetTimer()
    {
        current = duration;
    }

    /// <summary>
    /// Sets current to 0
    /// </summary>
    public void DeactivateTimer()
    {
        current = 0f;
    }

    /// <summary>
    /// checks if current > 0
    /// </summary>
    public bool TimerActive()
    {
        return current > 0;
    }

    /// <summary>
    /// Decrements timer by Time.deltatime
    /// </summary>
    public void CountDown()
    {
        current -= Time.deltaTime;
    }
}