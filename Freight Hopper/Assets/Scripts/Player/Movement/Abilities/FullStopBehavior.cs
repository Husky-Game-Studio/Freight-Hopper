using UnityEngine;
using UnityEngine.Rendering;

public class FullStopBehavior : AbilityBehavior
{
    [SerializeField] private Timer fullstopDuration = new Timer(1);
    [SerializeField] private Volume fullstopEffect;
    [SerializeField] private Gravity playerGravity;
    public override void Action()
    {
        fullstopDuration.CountDown(Time.fixedDeltaTime);

        float ratio = fullstopDuration.Current / fullstopDuration.Duration;
        fullstopEffect.weight = Mathf.Sin(Mathf.PI * ratio);
        rb.velocity = Vector3.zero;
    }

    public bool FullStopFinished()
    {
        return !fullstopDuration.TimerActive();
    }

    public override void ExitAction()
    {
        base.ExitAction();
        playerGravity.EnableGravity();
    }

    public override void EntryAction()
    {
        fullstopDuration.ResetTimer();
        soundManager.Play("Fullstop");
        playerGravity.DisableGravity();
    }
}