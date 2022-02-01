using UnityEngine;

public class JumpBehavior : AbilityBehavior
{
    [SerializeField] private float minJumpHeight = 2f;
    [SerializeField] private float holdingJumpForceMultiplier = 5f;
    [SerializeField] private RigidbodyLinker rigidbodyLinker;
    [SerializeField] private CollisionManagement collisionManager;

    public Timer jumpHoldingTimer = new Timer(0.5f);
    public Timer coyoteeTimer = new Timer(0.5f);
    public Timer jumpBufferTimer = new Timer(0.3f);
    public float JumpHeight => minJumpHeight;

    /// <summary>
    /// Same as jump but inputs the users current jump height
    /// </summary>
    public void Jump()
    {
        Jump(this.JumpHeight);
    }

    /// <summary>
    /// Physics behavior for the initial press of jump button
    /// </summary>
    public void Jump(float height)
    {
        Vector3 gravity = CustomGravity.GetGravity(rb.position, out Vector3 upAxis);

        if (Vector3.Dot(rb.velocity, upAxis) < 0)
        {
            rb.velocity = rb.velocity.ProjectOnContactPlane(upAxis);
        }

        // Basic physics, except the force required to reach this height may not work if we consider holding space
        // That and considering that physics works in timesteps.
        float jumpForce = Mathf.Sqrt(2f * gravity.magnitude * height);

        // Upward bias for sloped jumping
        Vector3 jumpDirection = (collisionManager.ContactNormal.old + upAxis).normalized;

        // Considers velocity when jumping on slopes and the slope angle
        float alignedSpeed = Vector3.Dot(rb.velocity, jumpDirection);
        if (alignedSpeed > 0)
        {
            jumpForce = Mathf.Max(jumpForce - alignedSpeed, 0);
        }

        // Actual jump itself
        rb.AddForce(jumpForce * upAxis, ForceMode.VelocityChange);
        if (rigidbodyLinker.ConnectedRb.current != null)
        {
            rb.AddForce(Vector3.Project(rigidbodyLinker.ConnectionVelocity.current, upAxis), ForceMode.VelocityChange);
        }

        soundManager.Play("Jump");
    }

    public override void EntryAction()
    {
        Jump();
    }

    public override void Action()
    {
        rb.AddForce(CustomGravity.GetUpAxis(rb.position) * holdingJumpForceMultiplier, ForceMode.Acceleration);
    }
}