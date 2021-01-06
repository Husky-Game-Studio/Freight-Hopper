using System;
using UnityEngine;

public class JumpBehavior : MonoBehaviour {
    private UserInput input;
    private Transform playerTransform;
    private Rigidbody rb;
    private bool isGrounded;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCount;
    private float hangTime = 0.2f;
    private float hangTimeCount;
    private bool justUngrounded;

    // Variable to chech if we are allowed to jump
    private bool jump;

    // Constructs the variables when the game starts
    private void Awake() {
        input = GetComponent<UserInput>();
        playerTransform = GetComponent<Transform>();
        isGrounded = false;
        rb = GetComponent<Rigidbody>();
        jump = false;
        justUngrounded = false;
    }

    // When character collides with another object it gets called
    private void OnCollisionEnter(UnityEngine.Collision other) {
        if (other.gameObject.CompareTag("landable")) {
            isGrounded = true;
        }
    }

    // when character exits collision with landable it sets grounded to false so we wouldn't be able to jump in air
    private void OnCollisionExit(UnityEngine.Collision other) {
        if (other.gameObject.CompareTag("landable")) {
            isGrounded = false;
            justUngrounded = true;
        }
    }

    // Updates every frame
    private void Update() {
        // Jump buffering
        if (input.Jumped()) {
            jumpBufferCount = jumpBufferTime;
        } else {
            jumpBufferCount -= Time.deltaTime;
        }
        // Hang Time
        if (justUngrounded) {
            justUngrounded = false;
            if (hangTimeCount <= 0f) {
                hangTimeCount = hangTime;
                isGrounded = true;
            } else {
                hangTimeCount -= Time.deltaTime;
            }
        }
        if (jumpBufferCount > 0f && hangTimeCount > 0f) {
            jump = true;
            jumpBufferCount = 0f;
            hangTimeCount = 0f;
        }
    }

    private void FixedUpdate() {
        // Jump code
        if (jump) {
            rb.AddForce(new Vector3(0f, 10f, 0f), ForceMode.Impulse);
            isGrounded = false;
            jump = false;
            justUngrounded = false;
        }
    }
}