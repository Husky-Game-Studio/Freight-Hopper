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
        rigidbody1 = GetComponent<Rigidbody>();
        jumpBehavior = GetComponent<JumpBehavior>();
    }

    private void Update() {
        delay.CountDown();
        if (UserInput.Input.GroundPound() && !delay.TimerActive() && groundPoundPossible) {
            groundPound = true;
            groundPoundPossible = false;
            delay.ResetTimer();
        }

        if (jumpBehavior.IsGrounded || !UserInput.Input.GroundPound()) {
            groundPoundPossible = true;
            groundPound = false;
        }
    }

    private void FixedUpdate() {
        if (groundPound) {
            rigidbody1.AddForce(Vector3.down * downwardsForce);
        }
    }
}