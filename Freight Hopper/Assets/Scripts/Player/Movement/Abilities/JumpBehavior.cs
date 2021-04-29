using UnityEngine;

public class JumpBehavior : AbilityBehavior
{
    [SerializeField] private float minJumpHeight = 2f;
    [SerializeField] private float holdingJumpForceMultiplier = 5f;

    public float JumpHeight => minJumpHeight;

    /// <summary>
    /// Same as jump but inputs the users current jump height
    /// </summary>
    public void Jump()
    {
        Jump(minJumpHeight);
    }

    /// <summary>
    /// Physics behavior for the initial press of jump button
    /// </summary>
    public void Jump(float height)
    {
        Vector3 gravity = CustomGravity.GetGravity(playerRb.position, out Vector3 upAxis);

        if (Vector3.Dot(playerRb.velocity, upAxis) < 0)
        {
            playerRb.velocity = playerRb.velocity.ProjectOnContactPlane(upAxis);
        }

        // Basic physics, except the force required to reach this height may not work if we consider holding space
        // That and considering that physics works in timesteps.
        float jumpForce = Mathf.Sqrt(2f * gravity.magnitude * height);

        // Upward bias for sloped jumping
        Vector3 jumpDirection = (playerCM.ContactNormal.old + upAxis).normalized;

        // Considers velocity when jumping on slopes and the slope angle
        float alignedSpeed = Vector3.Dot(playerRb.velocity, jumpDirection);
        if (alignedSpeed > 0)
        {
            jumpForce = Mathf.Max(jumpForce - alignedSpeed, 0);
        }

        // Actual jump itself
        playerRb.AddForce(jumpForce * upAxis, ForceMode.VelocityChange);
        if (playerCM.rigidbodyLinker.ConnectedRb.current != null)
        {
            playerRb.AddForce(Vector3.Project(playerCM.rigidbodyLinker.ConnectionVelocity.current, upAxis), ForceMode.VelocityChange);
        }

        playerSM.Play("Jump");
    }

    public override void EntryAction()
    {
        Jump();
    }

    public override void Action()
    {
        playerRb.AddForce(CustomGravity.GetUpAxis(playerRb.position) * holdingJumpForceMultiplier, ForceMode.Acceleration);
    }
}