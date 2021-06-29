using UnityEngine;

public class UpwardDashBehavior : AbilityBehavior
{
    [SerializeField, ReadOnly] private Vector3 storedVelocity;

    [SerializeField] private float initialUpwardsForce;
    [SerializeField] private float consistentForce;
    public Timer duration = new Timer(1);

    public override void EntryAction()
    {
        storedVelocity = playerPM.rb.velocity;
        duration.ResetTimer();
        playerSM.Play("UpwardDashEntry");
        playerPM.rb.velocity = Vector3.zero;
        playerPM.rb.AddForce(playerPM.collisionManager.ValidUpAxis * initialUpwardsForce, ForceMode.VelocityChange);
    }

    public override void Action()
    {
        playerSM.Play("UpwardDashTick");
        duration.CountDownFixed();
        playerPM.rb.AddForce(playerPM.collisionManager.ValidUpAxis * consistentForce, ForceMode.VelocityChange);
        playerPM.rb.velocity = Vector3.Project(playerPM.rb.velocity, playerPM.collisionManager.ValidUpAxis);
    }

    public override void ExitAction()
    {
        base.ExitAction();
        playerSM.Stop("UpwardDashTick");
        playerSM.Play("UpwardDashExit");
        playerPM.rb.velocity = storedVelocity;
        storedVelocity = Vector3.zero;
    }
}