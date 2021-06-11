using UnityEngine;

public class GroundPoundBehavior : AbilityBehavior
{
    [ReadOnly, SerializeField] private Var<float> increasingForce = new Var<float>(1, 1);

    [SerializeField] private float deltaIncreaseForce = 0.01f;
    [SerializeField] private float initialBurstForce = 20;
    [SerializeField] private float downwardsForce = 10;
    [SerializeField] private float slopeDownForce = 500;
    [SerializeField] private float groundFrictionReductionPercent = 0.95f;
    public float FrictionReduction => groundFrictionReductionPercent;

    public override void EntryAction()
    {
        playerSM.Play("GroundPoundBurst");
        Vector3 upAxis = playerCM.ValidUpAxis;
        if (Vector3.Dot(Vector3.Project(playerRb.velocity, upAxis), playerRb.transform.up) > 0)
        {
            playerRb.velocity = Vector3.ProjectOnPlane(playerRb.velocity, upAxis);
        }
        playerRb.AddForce(-upAxis * initialBurstForce, ForceMode.VelocityChange);
    }

    public override void Action()
    {
        playerSM.Play("GroundPoundTick");
        Vector3 upAxis = playerCM.ValidUpAxis;
        Vector3 direction = -upAxis;
        if (playerCM.IsGrounded.current)
        {
            Vector3 acrossSlope = Vector3.Cross(upAxis, playerCM.ContactNormal.current);
            Vector3 downSlope = Vector3.Cross(acrossSlope, playerCM.ContactNormal.current);
            direction = downSlope;
            if (!playerCM.IsGrounded.old)
            {
                playerRb.AddForce(direction * playerCM.Velocity.old.magnitude, ForceMode.VelocityChange);
            }
            direction *= slopeDownForce;
        }
        else
        {
            direction *= downwardsForce;
        }

        playerRb.AddForce(direction * increasingForce.current, ForceMode.Acceleration);
        increasingForce.current += deltaIncreaseForce * Time.fixedDeltaTime;
    }

    public override void ExitAction()
    {
        base.ExitAction();
        playerSM.Play("GroundPoundExit");
        increasingForce.RevertCurrent();
    }
}