using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JumpBehavior))]
public class UpwardDash : MonoBehaviour
{
    private bool upwardDashPossible = true;
    [SerializeField] private float initialUpwardsForce;
    [SerializeField] private float consistentForce;
    [SerializeField] private Timer delay = new Timer(0.5f);
    [SerializeField] private Timer dashDuration = new Timer(1);
    private bool upwardDash = false;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        delay.CountDown();
        dashDuration.CountDown();
        if (UserInput.Input.UpwardDash() && !delay.TimerActive() && upwardDashPossible)
        {
            upwardDash = true;
            upwardDashPossible = false;
            delay.ResetTimer();
        }

        if (GetComponent<JumpBehavior>().IsGrounded)
        {
            upwardDashPossible = true;
        }

        if (dashDuration.TimerActive())
        {
            rb.AddForce(Vector3.up * consistentForce);
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    private void FixedUpdate()
    {
        if (upwardDash)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * initialUpwardsForce, ForceMode.Impulse);
            upwardDash = false;
            dashDuration.ResetTimer();
            upwardDashPossible = false;
        }
    }
}