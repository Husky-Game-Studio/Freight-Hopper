using UnityEngine;

[System.Serializable]
public class GroundPound
{
    [ReadOnly, SerializeField] private bool groundPoundPossible = true;
    [ReadOnly, SerializeField] private bool groundPounding = false;
    [ReadOnly, SerializeField] private Var<float> increasingForce = new Var<float>(1, 1);
    [SerializeField] private float deltaIncreaseForce = 0.01f;
    [SerializeField] private float initialBurstForce = 20;
    [SerializeField] private float downwardsForce = 10;
    [SerializeField] private float slopeDownForce = 500;

    private CollisionManagement playerCollision;
    private Rigidbody rb;

    public void Initalize(Rigidbody rb, CollisionManagement playerCollision)
    {
        this.rb = rb;
        this.playerCollision = playerCollision;
    }

    public void TryGroundPound()
    {
        if (groundPoundPossible)
        {
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
            rb.AddForce(-CustomGravity.GetUpAxis(rb.position) * initialBurstForce, ForceMode.VelocityChange);
            groundPounding = true;
            groundPoundPossible = false;
        }
    }

    public void GroundPounding()
    {
        // Checks if ground pound is possible
        if (!UserInput.Input.GroundPoundTriggered())
        {
            groundPoundPossible = true;
            groundPounding = false;
        }
        Vector3 upAxis = CustomGravity.GetUpAxis(rb.position);
        if (groundPounding && playerCollision.ContactNormal.current != upAxis)
        {
            Vector3 direction = -upAxis;
            if (playerCollision.IsGrounded.current)
            {
                Vector3 acrossSlope = Vector3.Cross(upAxis, playerCollision.ContactNormal.current);
                Vector3 downSlope = Vector3.Cross(acrossSlope, playerCollision.ContactNormal.current);
                direction = downSlope;
                direction *= slopeDownForce;
            }
            else
            {
                direction *= downwardsForce;
            }

            rb.AddForce(direction * increasingForce.current, ForceMode.Acceleration);
            increasingForce.current += deltaIncreaseForce;
        }
        else
        {
            groundPounding = false;
            increasingForce.RevertCurrent();
        }
    }
}