using UnityEngine;
using UnityEngine.Rendering;

public class FullStopBehavior : AbilityBehavior
{
    [SerializeField] private Timer fullstopDuration = new Timer(1);
    [SerializeField] private Volume fullstopEffect;

    public override void Action()
    {
        fullstopDuration.CountDownFixed();

        float ratio = fullstopDuration.current / fullstopDuration.duration;
        fullstopEffect.weight = Mathf.Sin(Mathf.PI * ratio);
        physicsManager.rb.velocity = Vector3.zero;
    }

    public bool FullStopFinished()
    {
        return !fullstopDuration.TimerActive();
    }

    public override void ExitAction()
    {
        base.ExitAction();
        Player.Instance.GetComponent<PhysicsManager>().gravity.EnableGravity();
    }

    public override void EntryAction()
    {
        fullstopDuration.ResetTimer();
        soundManager.Play("Fullstop");
        Player.Instance.GetComponent<PhysicsManager>().gravity.DisableGravity();
    }
}