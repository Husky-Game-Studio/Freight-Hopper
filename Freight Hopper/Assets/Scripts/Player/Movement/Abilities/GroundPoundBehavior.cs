using UnityEngine;

public class GroundPoundBehavior : AbilityBehavior
{
    [ReadOnly, SerializeField] private Current<float> increasingForce = new Current<float>(1);

    [SerializeField] private float deltaIncreaseForce = 0.01f;
    [SerializeField] private float angleToBeConsideredFlat;
    [SerializeField] private float initialBurstForce = 20;
    [SerializeField] private float downwardsForce = 10;
    [SerializeField] private float slopeDownForce = 500;
    [SerializeField] private float groundFrictionReductionPercent = 0.95f;
    public float FrictionReduction => groundFrictionReductionPercent;

    public bool FlatSurface =>
         physicsManager.collisionManager.IsGrounded.current
            && Vector3.Angle(physicsManager.collisionManager.ValidUpAxis, physicsManager.collisionManager.ContactNormal.current) < angleToBeConsideredFlat;

    public override void EntryAction()
    {
        soundManager.Play("GroundPoundBurst");
        Vector3 upAxis = physicsManager.collisionManager.ValidUpAxis;
        if (Vector3.Dot(Vector3.Project(physicsManager.rb.velocity, upAxis), physicsManager.rb.transform.up) > 0)
        {
            physicsManager.rb.velocity = Vector3.ProjectOnPlane(physicsManager.rb.velocity, upAxis);
        }
        physicsManager.rb.AddForce(-upAxis * initialBurstForce, ForceMode.VelocityChange);
    }

    public override void Action()
    {
        soundManager.Play("GroundPoundTick");
        Vector3 upAxis = physicsManager.collisionManager.ValidUpAxis;
        Vector3 direction = -upAxis;
        if (physicsManager.collisionManager.IsGrounded.current)
        {
            Vector3 acrossSlope = Vector3.Cross(upAxis, physicsManager.collisionManager.ContactNormal.current);
            Vector3 downSlope = Vector3.Cross(acrossSlope, physicsManager.collisionManager.ContactNormal.current);
            direction = downSlope;
            if (!physicsManager.collisionManager.IsGrounded.old)
            {
                physicsManager.rb.AddForce(direction * physicsManager.collisionManager.Velocity.old.magnitude, ForceMode.VelocityChange);
            }
            direction *= slopeDownForce;
        }
        else
        {
            direction *= downwardsForce;
        }

        physicsManager.rb.AddForce(direction * increasingForce.value, ForceMode.Acceleration);
        increasingForce.value += deltaIncreaseForce * Time.fixedDeltaTime;
    }

    public override void ExitAction()
    {
        base.ExitAction();
        soundManager.Play("GroundPoundExit");
        increasingForce.Reset();
    }
}