using System;
using UnityEngine;

public class JumpBehavior : MonoBehaviour {

    [Serializable]
    private struct Timer {
        public float duration;
        public float current;

        public Timer(float duration) {
            this.duration = duration;
            current = 0f;
        }

        public void resetTimer() {
            current = duration;
        }

        public void deactivateTimer() {
            current = 0f;
        }

        public bool timerActive() {
            return current > 0;
        }

        public void countDown() {
            current -= Time.deltaTime;
        }
    }

    private UserInput input;
    private Transform playerTransform;
    private Rigidbody rb;
    private bool isGrounded;
    private Timer jumpBuffer = new Timer(0.2f);
    private Timer hangTime = new Timer(1f);

    // Variable to chech if we are allowed to jump
    private bool jump;

    private bool canJump;

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
            canJump = true;
        }
    }

    // when character exits collision with landable it sets grounded to false so we wouldn't be able to jump in air
    private void OnCollisionExit(UnityEngine.Collision other) {
        if (other.gameObject.CompareTag("landable")) {
            isGrounded = false;
            hangTime.resetTimer();
        }
    }

    // Updates every frame
    private void Update() {
        // Jump buffering
        if (input.Jumped()) {
            jumpBuffer.resetTimer();
        } else {
            jumpBuffer.countDown();
        }
        //Hang Time
        if (!hangTime.timerActive() && !isGrounded) {
            canJump = false;
        } else {
            /*
            hangTime.countDown();
        */
        }
        hangTime.countDown();

        if (jumpBuffer.timerActive() && canJump) {
            jump = true;
            jumpBuffer.deactivateTimer();
        }
    }

    private void FixedUpdate() {
        // Jump code
        if (jump) {
            rb.AddForce(new Vector3(0f, 10f, 0f), ForceMode.Impulse);
            isGrounded = false;
            jump = false;
            canJump = false;
        }
    }
}