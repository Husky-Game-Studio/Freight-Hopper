using System;

[Serializable]
// Like the timer, but it automatically resets and calls functions for you on reset
public class AutomaticTimer : Timer
{
    private event Action ReachedZeroEvent;

    // Sets current to duration
    public AutomaticTimer(float duration) : base(duration)
    {
        this.duration = duration;
        current = duration;
    }

    // Listen to ReachedZeroEvent
    public void Subscribe(Action function) => ReachedZeroEvent += function;

    // Unlisten to ReachedZeroEvent, do this if you don't like memory leaks
    public void Unsubscribe(Action function) => ReachedZeroEvent -= function;

    ~AutomaticTimer()
    {
    }

    public override void ResetTimer()
    {
        base.ResetTimer();
    }

    // Counts down, resets when below 0, and invokes reset event
    public void Update(float amount)
    {
        base.CountDown(amount);
        if (current <= 0)
        {
            ResetTimer();
            ReachedZeroEvent.Invoke();
        }
    }
}