using UnityEngine;

[System.Serializable]
public class JumpBehavior : MonoBehaviour
{
    [SerializeField] private Timer jumpBuffer = new Timer(0.3f);
    [SerializeField] private Timer jumpHoldingPeriod = new Timer(0.5f);
    [SerializeField] private Timer coyoteTime = new Timer(0.5f);
    [SerializeField] private float minJumpHeight = 2f;
    [SerializeField] private float holdingJumpForceMultiplier = 5f;

    private Rigidbody rb;
    private CollisionManagement playerCollision;
    private AudioSource jumpSound;

    public float JumpHeight => minJumpHeight;
    public bool CanJump => coyoteTime.TimerActive();
    public bool JumpBufferActive => jumpBuffer.TimerActive();
    public bool IsJumping => jumpHoldingPeriod.TimerActive();


    public void Initialize(Rigidbody rb, CollisionManagement playerCollision, AudioSource jumpSound)
    {
        this.rb = rb;
        this.playerCollision = playerCollision;
        this.jumpSound = jumpSound;
    }

    private void OnEnable()
    {
        UserInput.Input.JumpInput += jumpBuffer.ResetTimer;
        playerCollision.Landed += coyoteTime.ResetTimer;
    }

    private void OnDisable()
    {
        UserInput.Input.JumpInput -= jumpBuffer.ResetTimer;
        playerCollision.Landed -= coyoteTime.ResetTimer;
    }

    // If in fall state //
    public void DecrementTimers()
    {
        coyoteTime.CountDownFixed();
        jumpBuffer.CountDownFixed();
    }

    // Every LateFixedUpdate checks for jump buffering and if player is holding space
    public void Jumping()
    {
        // If landed state and jump buffer active //
        // Jump buffer jumping
        if (jumpBuffer.TimerActive() && playerCollision.IsGrounded.current)
        {
            Jump(minJumpHeight);
        }


        // If in jumping state //
        jumpHoldingPeriod.CountDownFixed();
        if (jumpHoldingPeriod.TimerActive() && !playerCollision.IsGrounded.current)
        {
            if (!UserInput.Input.Jump())
            {
                jumpHoldingPeriod.DeactivateTimer();
            }
            else
            {
                rb.AddForce(CustomGravity.GetUpAxis(rb.position) * holdingJumpForceMultiplier, ForceMode.Acceleration);
            }
        }
    }

    // Jumps to minimum height
    public void Jump(float height)
    {
        jumpBuffer.DeactivateTimer();
        coyoteTime.DeactivateTimer();
        jumpHoldingPeriod.ResetTimer();

        if (!jumpSound.isPlaying)
        {
            jumpSound.Play();
        }
        Vector3 gravity = CustomGravity.GetGravity(rb.position, out Vector3 upAxis);

        if (Vector3.Dot(rb.velocity, upAxis) < 0)
        {
            rb.velocity = CollisionManagement.ProjectOnContactPlane(rb.velocity, upAxis);
        }

        // Basic physics, except the force required to reach this height may not work if we consider holding space
        // That and considering that physics works in timesteps.
        float jumpForce = Mathf.Sqrt(2f * gravity.magnitude * height);
        Camera.main.GetComponent<CameraDrag>().CollidDrag(upAxis);

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

    // Jumps if conditions are correct
    /*public void TryJump()
    {
        if (playerCollision.IsGrounded.current || coyoteTime.TimerActive())
        {
            Jump(minJumpHeight);
        }
    }*/
}