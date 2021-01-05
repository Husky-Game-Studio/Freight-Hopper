using System;
using UnityEngine;

public class JumpBehavior : MonoBehaviour {
    private UserInput input;
    private Transform playerTransform;
    private Rigidbody rb;
    private bool isGrounded;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCount;

    // Variable to chech if we are allowed to jump
    private bool jump;

    // Constructs the variables when the game starts
    private void Awake() {
        input = GetComponent<UserInput>();
        playerTransform = GetComponent<Transform>();
        isGrounded = false;
        rb = GetComponent<Rigidbody>();
        jump = false;
    }

    // When character collides with another object it gets called
    private void OnCollisionEnter(UnityEngine.Collision other) {
        if (other.gameObject.CompareTag("landable")) {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(UnityEngine.Collision other) {
        if (other.gameObject.CompareTag("landable")) {
            isGrounded = false;
        }
    }

    // Updates every frame
    private void Update() {
        if (input.Jumped()) {
            jumpBufferCount = jumpBufferTime;
        } else {
            jumpBufferCount -= Time.deltaTime;
        }
        if (jumpBufferCount > 0f && isGrounded) {
            jump = true;
            jumpBufferCount = 0;
        }
    }

    private void FixedUpdate() {
        // Jump code
        //////////////////////////////////////////////////////////////////
        if (jump) {
            rb.AddForce(new Vector3(0f, 10f, 0f), ForceMode.Impulse);
            isGrounded = false;
            jump = false;
        }
    }
}