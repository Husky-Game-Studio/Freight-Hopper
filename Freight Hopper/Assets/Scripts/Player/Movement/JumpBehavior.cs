using UnityEngine;

[System.Serializable]
public class JumpBehavior : MonoBehaviour
{
    [SerializeField] private Timer jumpBuffer = new Timer(0.3f);
    [SerializeField] private Timer jumpHoldingPeriod = new Timer(0.5f);
    [SerializeField] public Timer coyoteTime = new Timer(0.5f);
    [SerializeField] private float minJumpHeight = 2f;
    [SerializeField] private float holdingJumpForceMultiplier = 5f;

    private Rigidbody rb;
    private CollisionManagement playerCollision;
    private AudioSource jumpSound;

    public float JumpHeight => minJumpHeight;
    public bool CanJump => coyoteTime.TimerActive();

    public void Initialize(Rigidbody rb, CollisionManagement playerCollision, AudioSource jumpSound)
    {
        this.rb = rb;
        this.playerCollision = playerCollision;
        this.jumpSound = jumpSound;
    }

    private void OnEnable()
    {
        UserInput.Input.JumpInput += TryJump;
        playerCollision.Landed += coyoteTime.ResetTimer;
        playerCollision.CollisionDataCollected += Jumping;
    }

    private void OnDisable()
    {
        UserInput.Input.JumpInput -= TryJump;
        playerCollision.Landed -= coyoteTime.ResetTimer;
        playerCollision.CollisionDataCollected -= Jumping;
    }

    // Every LateFixedUpdate checks for jump buffering and if player is holding space
    public void Jumping()
    {
        if (!playerCollision.IsGrounded.current)
        {
            coyoteTime.CountDownFixed();
            jumpBuffer.CountDownFixed();
            jumpHoldingPeriod.CountDownFixed();
        }

        // Jump buffer jumping
        if (jumpBuffer.TimerActive() && playerCollision.IsGrounded.current)
        {
            Jump(minJumpHeight);
        }

        // This lowers your gravity while you are holding space for the timer period while you are holding jump
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
    public void TryJump()
    {
        jumpBuffer.ResetTimer();
        if (playerCollision.IsGrounded.current || coyoteTime.TimerActive())
        {
            Jump(minJumpHeight);
        }
    }

    // Gabe: need to see if player collides with ground
    /*public bool getIsGroundedEvent()
    {
        return playerCollision.getIsGroundedEvent();
    }*/
}