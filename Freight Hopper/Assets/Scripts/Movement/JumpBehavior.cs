using System;
using UnityEngine;

public class JumpBehavior : MonoBehaviour
{
    private Transform playerTransform;
    private Rigidbody rb;
    private bool isGrounded;

    // isGrounded should sometime be seperated from the jump behavior. Other scripts depend on it.
    public bool IsGrounded => isGrounded;

    [SerializeField] private Timer jumpBuffer = new Timer(0.2f);
    [SerializeField] private Timer hangTime = new Timer(0.2f);
    [SerializeField] private float jumpForce = 10;
    public float JumpForce => jumpForce;

    // Variable to chech if we are allowed to jump
    private bool jump;

    private bool canJump;
    public bool CanJump => canJump;

    // Constructs the variables when the game starts
    private void Awake()
    {
        playerTransform = GetComponent<Transform>();
        isGrounded = false;
        rb = GetComponent<Rigidbody>();
        jump = false;
    }

    // When character collides with another object it gets called
    private void OnCollisionEnter(UnityEngine.Collision other)
    {
        if (other.gameObject.CompareTag("landable"))
        {
            isGrounded = true;
            canJump = true;
        }
    }

    // when character exits collision with landable it sets grounded to false so we wouldn't be able to jump in air
    private void OnCollisionExit(UnityEngine.Collision other)
    {
        if (other.gameObject.CompareTag("landable"))
        {
            isGrounded = false;
            hangTime.ResetTimer();
        }
    }

    // Updates every frame
    private void Update()
    {
        // Jump buffering
        if (UserInput.Input.Jump())
        {
            jumpBuffer.ResetTimer();
        }
        else
        {
            jumpBuffer.CountDown();
        }
        //Hang Time
        if (!hangTime.TimerActive() && !isGrounded)
        {
            canJump = false;
        }
        else
        {
            /*
            hangTime.countDown();
        */
        }
        hangTime.CountDown();

        if (jumpBuffer.TimerActive() && canJump)
        {
            jump = true;
            jumpBuffer.DeactivateTimer();
        }
    }

    private void FixedUpdate()
    {
        // Jump code
        if (jump)
        {
            rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
            isGrounded = false;
            jump = false;
            canJump = false;
        }
    }
}