using UnityEngine;

[System.Serializable]
public class JumpBehavior : MonoBehaviour
{
    [SerializeField] private float minJumpHeight = 2f;
    [SerializeField] private float holdingJumpForceMultiplier = 5f;

    private Rigidbody rb;
    private CollisionManagement playerCollision;

    public float JumpHeight => minJumpHeight;

    public void Initialize(Rigidbody rb, CollisionManagement playerCollision)
    {
        this.rb = rb;
        this.playerCollision = playerCollision;
    }

    // Physics behavior for holding the jump button
    public void HoldingJump()
    {
        rb.AddForce(CustomGravity.GetUpAxis(rb.position) * holdingJumpForceMultiplier, ForceMode.Acceleration);
    }

    /// <summary>
    /// Same as jump but inputs the users current jump height
    /// </summary>
    public void Jump()
    {
        Jump(minJumpHeight);
    }

    // Physics behavior for the initial press of jump button
    public void Jump(float height)
    {
        Vector3 gravity = CustomGravity.GetGravity(rb.position, out Vector3 upAxis);

        if (Vector3.Dot(rb.velocity, upAxis) < 0)
        {
            rb.velocity = CollisionManagement.ProjectOnContactPlane(rb.velocity, upAxis);
        }

        // Basic physics, except the force required to reach this height may not work if we consider holding space
        // That and considering that physics works in timesteps.
        float jumpForce = Mathf.Sqrt(2f * gravity.magnitude * height);

        // Upward bias for sloped jumping
        Vector3 jumpDirection = (playerCollision.ContactNormal.old + upAxis).normalized;

        // Considers velocity when jumping on slopes and the slope angle
        float alignedSpeed = Vector3.Dot(rb.velocity, jumpDirection);
        if (alignedSpeed > 0)
        {
            jumpForce = Mathf.Max(jumpForce - alignedSpeed, 0);
        }

        // Actual jump itself
        rb.AddForce(jumpForce * upAxis, ForceMode.VelocityChange);
        if (playerCollision.rigidbodyLinker.ConnectedRb.old != null)
        {
            rb.AddForce(Vector3.Project(playerCollision.rigidbodyLinker.ConnectionVelocity.old, upAxis), ForceMode.VelocityChange);
        }
    }
}