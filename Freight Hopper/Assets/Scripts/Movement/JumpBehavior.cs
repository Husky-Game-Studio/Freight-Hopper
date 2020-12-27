using System;
using UnityEngine;

public class JumpBehavior : MonoBehaviour {
    private UserInput input;
    private Transform transform;
    private Rigidbody rb;
    private bool isGrounded;

    private void Awake() {
        input = GetComponent<UserInput>();
        transform = GetComponent<Transform>();
        isGrounded = true;
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(UnityEngine.Collision other) {
        if (other.gameObject.tag == "landable") {
            isGrounded = true;
        }
    }

    private void FixedUpdate() {
        if (input.Jumped() && isGrounded) {
            rb.AddForce(new Vector3(0f, 5f, 0f), ForceMode.Impulse);
            isGrounded = false;
        }
    }
}