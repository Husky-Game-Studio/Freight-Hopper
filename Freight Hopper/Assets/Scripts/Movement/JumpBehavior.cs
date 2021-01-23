using System;
using UnityEngine;

public class JumpBehavior : MonoBehaviour
{
    private Transform playerTransform;
    private Rigidbody rb;
    private bool isGrounded;
    private bool midJump;

    // isGrounded should sometime be seperated from the jump behavior. Other scripts depend on it.
    public bool IsGrounded => isGrounded;

    [SerializeField] private Timer jumpBuffer = new Timer(0.2f);
    [SerializeField] private Timer hangTime = new Timer(0.2f);
    [SerializeField] private Timer jumpTimer = new Timer(0.1f);
    [SerializeField] private float jumpForce = 5f;
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
        midJump = false;
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
        if (jump || midJump)
        {
            Jump();
        }
    }

    // When called makes character jump
    public void Jump()
    {
        if (midJump == false)
        {
            jumpTimer.ResetTimer();
        }

        if (jumpTimer.TimerActive() && UserInput.Input.Jump())
        {
            Camera.main.GetComponent<CameraDrag>().CollidDrag(Vector3.up);
            jumpTimer.CountDownFixed();
            rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
            midJump = true;
        }
        else
        {
            jumpTimer.DeactivateTimer();
            midJump = false;
        }

        isGrounded = false;
        jump = false;
        canJump = false;
    }
}