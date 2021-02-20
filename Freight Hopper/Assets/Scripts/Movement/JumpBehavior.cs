using System;
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
    [SerializeField] private float jumpHeight = 5f;

    [SerializeField, Range(0, 1000)] private float percentGravityDifferenceWhenHoldingSpace;

    public float JumpHeight => jumpHeight;

    private void OnEnable()
    {
        UserInput.JumpInput += TryJump;
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
        if (!playerCollision.IsGrounded)
        {
            coyoteTime.CountDown();
            jumpBuffer.CountDown();
            jumpHoldingPeriod.CountDown();
        }
    }

    private void FixedUpdate()
    {
        if (jumpBuffer.TimerActive() && playerCollision.IsGrounded)
        {
            //Debug.Log("Jump buffer timer active and grounded");
            Jump(jumpHeight);
        }
        if (jumpHoldingPeriod.TimerActive() && !playerCollision.IsGrounded)
        {
            if (UserInput.Input.Jump())
            {
                Debug.Log("Setting scale");
                gravity.scale.SetCurrent(gravity.scale.old / (percentGravityDifferenceWhenHoldingSpace / 100));
            }
            else
            {
                Debug.Log("Reverting current ");
                gravity.scale.RevertCurrent();
                jumpHoldingPeriod.DeactivateTimer();
            }
        }
        else
        {
            Debug.Log("Reverting current ");
            gravity.scale.RevertCurrent();
        }
    }

    public void Jump(float height)
    {
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
        rb.AddForce(jumpForce * gravity.Direction, ForceMode.VelocityChange);
        gravity.scale.SetCurrent(gravity.scale.old / (percentGravityDifferenceWhenHoldingSpace / 100));
    }

    // When called makes character jump
    private void TryJump()
    {
        jumpBuffer.ResetTimer();
        if (playerCollision.IsGrounded || coyoteTime.TimerActive())
        {
            //Debug.Log("On ground: " + playerCollision.IsGrounded);
            //Debug.Log("Coyote timer active: " + coyoteTime.TimerActive());
            Jump(jumpHeight);
        }
    }
}