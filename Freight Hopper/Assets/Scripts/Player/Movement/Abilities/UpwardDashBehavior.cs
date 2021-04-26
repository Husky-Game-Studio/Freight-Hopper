using UnityEngine;

public class UpwardDashBehavior : AbilityBehavior
{
    [SerializeField, ReadOnly] private Vector3 storedVelocity;

    [SerializeField] private float initialUpwardsForce;
    [SerializeField] private float consistentForce;
    //[SerializeField] private Timer delay = new Timer(0.5f);
    //[SerializeField] private Timer dashDuration = new Timer(1);

    public override void EntryAction()
    {
        storedVelocity = playerRb.velocity;
        playerRb.velocity = Vector3.zero;
        playerRb.AddForce(playerCM.ValidUpAxis * initialUpwardsForce, ForceMode.VelocityChange);
    }

    public override void Action()
    {
        playerRb.AddForce(playerCM.ValidUpAxis * consistentForce, ForceMode.VelocityChange);
        playerRb.velocity = new Vector3(0, playerRb.velocity.y, 0);
    }

    public override void ExitAction()
    {
        base.ExitAction();
        playerRb.velocity = storedVelocity;
        storedVelocity = Vector3.zero;
    }
}