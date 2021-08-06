using UnityEngine;

[System.Serializable]
public class Timer
{
    [SerializeField] private float duration;
    public float Duration => duration;
    [SerializeField, ReadOnly] private float current;
    public float Current => current;

    // Initailizes duration and sets current to 0
    public Timer(float duration)
    {
        this.duration = duration;
        current = 0f;
    }

    // Sets current to duration
    public void ResetTimer()
    {
        if (current != duration)
        {
            current = duration;
        }
    }

    // Sets current to 0
    public void DeactivateTimer()
    {
        current = 0f;
    }

    // Checks if current > 0
    public bool TimerActive()
    {
        return current > 0;
    }

    // Decrements timer by amount, usually Time.fixedDeltaTime or Time.deltaTime
    public void CountDown(float amount)
    {
        current -= amount;
    }
}