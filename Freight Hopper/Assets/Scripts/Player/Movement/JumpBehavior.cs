using UnityEngine;

[RequireComponent(typeof(CollisionCheck), typeof(Gravity))]
public class JumpBehavior : MonoBehaviour
{
    private Rigidbody rb;
    private CollisionCheck playerCollision;
    private Gravity gravity;
    private AudioSource jumpSound;

    [SerializeField] private Timer jumpBuffer = new Timer(0.3f);
    [SerializeField] private Timer jumpHoldingPeriod = new Timer(0.5f);
    [SerializeField] private Timer coyoteTime = new Timer(0.5f);
    [SerializeField] private float maxJumpHeight = 5f;
    [SerializeField] private float minJumpHeight = 2f;

    public float JumpHeight => minJumpHeight;
    [SerializeField] public bool CanJump => coyoteTime.TimerActive();

    private void OnEnable()
    {
        UserInput.JumpInput += TryJump;
    }

    private void OnDisable()
    {
        UserInput.JumpInput -= TryJump;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollision = GetComponent<CollisionCheck>();
        gravity = GetComponent<Gravity>();
        jumpSound = GetComponent<AudioSource>();
        playerCollision.Landed += coyoteTime.ResetTimer;
    }

    // Updates every frame
    private void Update()
    {
        if (!playerCollision.IsGrounded.current)
        {
            coyoteTime.CountDown();
            jumpBuffer.CountDown();
            jumpHoldingPeriod.CountDown();
        }
    }

    private void FixedUpdate()
    {
        if (jumpBuffer.TimerActive() && playerCollision.IsGrounded.old)
        {
            Jump(minJumpHeight);
        }
        if (jumpHoldingPeriod.TimerActive() && !playerCollision.IsGrounded.current)
        {
            if (UserInput.Input.Jump())
            {
                gravity.scale.SetCurrent((gravity.scale.old * minJumpHeight) / maxJumpHeight);
            }
            else
            {
                gravity.scale.RevertCurrent();
                jumpHoldingPeriod.DeactivateTimer();
            }
        }
        else
        {
            gravity.scale.RevertCurrent();
        }
    }

    public void Jump(float height)
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
        jumpBuffer.DeactivateTimer();
        coyoteTime.DeactivateTimer();
        jumpHoldingPeriod.ResetTimer();

        if (!jumpSound.isPlaying)
        {
            jumpSound.Play();
        }

        // Basic physics, except the force required to reach this height may not work if we consider holding space
        // That and considering that physics works in timesteps.
        float jumpForce = Mathf.Sqrt(-2f * Gravity.constant * gravity.scale.current * height);
        Camera.main.GetComponent<CameraDrag>().CollidDrag(gravity.Direction);

        // Upward bias for sloped jumping
        Vector3 jumpDirection = (playerCollision.ContactNormal + gravity.Direction).normalized;
        // Considers velocity when jumping on slopes
        float alignedSpeed = Vector3.Dot(rb.velocity, jumpDirection);
        if (alignedSpeed > 0)
        {
            jumpForce = Mathf.Max(jumpForce - alignedSpeed, 0);
        }

        rb.AddForce(jumpForce * gravity.Direction, ForceMode.VelocityChange);

        // Lower gravity when holding space making you go higher
        if (UserInput.Input.Jump())
        {
            gravity.scale.SetCurrent((gravity.scale.old * minJumpHeight) / maxJumpHeight);
        }
    }

    // When called makes character jump
    private void TryJump()
    {
        jumpBuffer.ResetTimer();
        if (playerCollision.IsGrounded.current || coyoteTime.TimerActive())
        {
            Jump(minJumpHeight);
        }
    }
}