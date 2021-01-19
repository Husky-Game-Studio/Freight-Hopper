using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JumpBehavior))]
public class GroundPound : MonoBehaviour {
    private bool groundPoundPossible = true;
    [SerializeField] private float downwardsForce;
    [SerializeField] private Timer delay = new Timer(0.5f);
    private bool groundPound = false;
    private JumpBehavior jumpBehavior;
    private Rigidbody rigidbody1;

    // Update is called once per frame
    private void Start() {
        // Gets the rigid body component and jump behaviour script
        rigidbody1 = GetComponent<Rigidbody>();
        jumpBehavior = GetComponent<JumpBehavior>();
    }

    private void Update() {
        delay.CountDown();
        // Checks if ground pound is possible
        if (UserInput.Input.GroundPoundTriggered() && !delay.TimerActive() && groundPoundPossible) {
            groundPound = true;
            groundPoundPossible = false;
            delay.ResetTimer();
        }
        // Checks if player is grounded and the button is pressed
        if (jumpBehavior.IsGrounded || !UserInput.Input.GroundPoundTriggered()) {
            groundPoundPossible = true;
            groundPound = false;
        }
    }

    private void FixedUpdate() {
        // Adds the ground pound force
        if (groundPound) {
            rigidbody1.AddForce(Vector3.down * downwardsForce);
        }
    }
}