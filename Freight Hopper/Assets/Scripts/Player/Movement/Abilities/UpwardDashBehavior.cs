using UnityEngine;

public class UpwardDashBehavior : AbilityBehavior
{
    [SerializeField, ReadOnly] private Vector3 storedVelocity;

    [SerializeField] private float initialUpwardsForce;
    [SerializeField] private float consistentForce;
    public Timer duration = new Timer(1);
    private CollisionManagement collisionManager;

    public override void Initialize()
    {
        base.Initialize();
        collisionManager = Player.Instance.modules.collisionManagement;
    }

    public override void EntryAction()
    {
        storedVelocity = rb.velocity;
        duration.ResetTimer();
        soundManager.Play("UpwardDashEntry");
        rb.velocity = Vector3.zero;
        rb.AddForce(collisionManager.ValidUpAxis * initialUpwardsForce, ForceMode.VelocityChange);
    }

    public override void Action()
    {
        soundManager.Play("UpwardDashTick");
        duration.CountDown(Time.fixedDeltaTime);
        rb.AddForce(collisionManager.ValidUpAxis * consistentForce, ForceMode.VelocityChange);
        rb.velocity = Vector3.Project(rb.velocity, collisionManager.ValidUpAxis);
    }

    public override void ExitAction()
    {
        base.ExitAction();
        soundManager.Stop("UpwardDashTick");
        soundManager.Play("UpwardDashExit");
        rb.velocity = storedVelocity;
        storedVelocity = Vector3.zero;
    }
}