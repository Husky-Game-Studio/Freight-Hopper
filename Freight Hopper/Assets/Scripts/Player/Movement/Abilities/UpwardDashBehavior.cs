using UnityEngine;

public class UpwardDashBehavior : AbilityBehavior
{
    [SerializeField, ReadOnly] private Vector3 storedVelocity;

    [SerializeField] private float initialUpwardsForce;
    [SerializeField] private float consistentForce;
    public Timer duration = new Timer(1);

    public override void EntryAction()
    {
        storedVelocity = physicsManager.rb.velocity;
        duration.ResetTimer();
        soundManager.Play("UpwardDashEntry");
        physicsManager.rb.velocity = Vector3.zero;
        physicsManager.rb.AddForce(physicsManager.collisionManager.ValidUpAxis * initialUpwardsForce, ForceMode.VelocityChange);
    }

    public override void Action()
    {
        soundManager.Play("UpwardDashTick");
        duration.CountDown(Time.fixedDeltaTime);
        physicsManager.rb.AddForce(physicsManager.collisionManager.ValidUpAxis * consistentForce, ForceMode.VelocityChange);
        physicsManager.rb.velocity = Vector3.Project(physicsManager.rb.velocity, physicsManager.collisionManager.ValidUpAxis);
    }

    public override void ExitAction()
    {
        base.ExitAction();
        soundManager.Stop("UpwardDashTick");
        soundManager.Play("UpwardDashExit");
        physicsManager.rb.velocity = storedVelocity;
        storedVelocity = Vector3.zero;
    }
}