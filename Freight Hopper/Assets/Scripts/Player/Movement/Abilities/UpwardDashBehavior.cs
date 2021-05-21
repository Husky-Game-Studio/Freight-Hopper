using UnityEngine;

public class UpwardDashBehavior : AbilityBehavior
{
    [SerializeField, ReadOnly] private Vector3 storedVelocity;

    [SerializeField] private float initialUpwardsForce;
    [SerializeField] private float consistentForce;
    [SerializeField] public Timer duration = new Timer(1);

    public override void EntryAction()
    {
        storedVelocity = playerRb.velocity;
        duration.ResetTimer();
        playerRb.velocity = Vector3.zero;
        playerRb.AddForce(playerCM.ValidUpAxis * initialUpwardsForce, ForceMode.VelocityChange);
    }

    public override void Action()
    {
        playerSM.Play("UpwardDashTick");
        duration.CountDownFixed();
        playerRb.AddForce(playerCM.ValidUpAxis * consistentForce, ForceMode.VelocityChange);
        playerRb.velocity = Vector3.Project(playerRb.velocity, playerCM.ValidUpAxis);
    }

    public override void ExitAction()
    {
        base.ExitAction();
        playerSM.Stop("UpwardDashTick");
        playerRb.velocity = storedVelocity;
        storedVelocity = Vector3.zero;
    }
}