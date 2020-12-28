using System;
using UnityEngine;

public class JumpBehavior : MonoBehaviour {
    private UserInput input;
    private Transform transform;
    private Rigidbody rb;
    private bool isGrounded;
    private bool jump;

    private void Awake() {
        input = GetComponent<UserInput>();
        transform = GetComponent<Transform>();
        isGrounded = true;
        rb = GetComponent<Rigidbody>();
        jump = false;
    }

    private void OnCollisionEnter(UnityEngine.Collision other) {
        if (other.gameObject.tag == "landable") {
            isGrounded = true;
        }
    }

    private void Update() {
        if (input.Jumped() && isGrounded) {
            jump = true;
        }
    }

    private void FixedUpdate() {
        if (jump) {
            rb.AddForce(new Vector3(0f, 5f, 0f), ForceMode.Impulse);
            isGrounded = false;
            jump = false;
        }
    }
}