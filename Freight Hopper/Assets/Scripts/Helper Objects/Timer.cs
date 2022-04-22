using UnityEngine;

[System.Serializable]
public class Timer
{
    [SerializeField] protected float duration;
    public float Duration => duration;
    [SerializeField, ReadOnly] protected float current = 0;
    public float Current => current;
    private bool useUnscaled = false;

    // Initailizes duration and sets current to 0
    public Timer(float duration)
    {
        this.duration = duration;
    }

    // Sets current to duration
    public virtual void ResetTimer()
    {
        if (current != duration)
        {
            current = duration;
        }
    }

    // Sets current to 0
    public virtual void DeactivateTimer()
    {
        current = 0f;
    }

    // Checks if current > 0
    public virtual bool TimerActive()
    {
        return current > 0;
    }

    // Decrements timer by amount, usually Time.fixedDeltaTime or Time.deltaTime
    public virtual void CountDown(float amount)
    {
        current -= amount;
    }
}