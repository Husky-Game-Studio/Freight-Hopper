using UnityEngine;

public class GroundPoundBehavior : AbilityBehavior
{
    [ReadOnly, SerializeField] private Current<float> increasingForce = new Current<float>(1);

    [SerializeField] private float deltaIncreaseForce = 0.01f;
    [SerializeField] private float initialBurstForce = 20;
    [SerializeField] private float downwardsForce = 10;
    [SerializeField] private float slopeDownForce = 500;
    [SerializeField] private float groundFrictionReductionPercent = 0.95f;
    public float FrictionReduction => groundFrictionReductionPercent;

    public override void EntryAction()
    {
        playerSM.Play("GroundPoundBurst");
        Vector3 upAxis = playerPM.collisionManager.ValidUpAxis;
        if (Vector3.Dot(Vector3.Project(playerPM.rb.velocity, upAxis), playerPM.rb.transform.up) > 0)
        {
            playerPM.rb.velocity = Vector3.ProjectOnPlane(playerPM.rb.velocity, upAxis);
        }
        playerPM.rb.AddForce(-upAxis * initialBurstForce, ForceMode.VelocityChange);
    }

    public override void Action()
    {
        playerSM.Play("GroundPoundTick");
        Vector3 upAxis = playerPM.collisionManager.ValidUpAxis;
        Vector3 direction = -upAxis;
        if (playerPM.collisionManager.IsGrounded.current)
        {
            Vector3 acrossSlope = Vector3.Cross(upAxis, playerPM.collisionManager.ContactNormal.current);
            Vector3 downSlope = Vector3.Cross(acrossSlope, playerPM.collisionManager.ContactNormal.current);
            direction = downSlope;
            if (!playerPM.collisionManager.IsGrounded.old)
            {
                playerPM.rb.AddForce(direction * playerPM.collisionManager.Velocity.old.magnitude, ForceMode.VelocityChange);
            }
            direction *= slopeDownForce;
        }
        else
        {
            direction *= downwardsForce;
        }

        playerPM.rb.AddForce(direction * increasingForce.value, ForceMode.Acceleration);
        increasingForce.value += deltaIncreaseForce * Time.fixedDeltaTime;
    }

    public override void ExitAction()
    {
        base.ExitAction();
        playerSM.Play("GroundPoundExit");
        increasingForce.Reset();
    }
}