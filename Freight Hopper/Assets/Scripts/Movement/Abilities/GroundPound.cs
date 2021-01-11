using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JumpBehavior))]
public class GroundPound : MonoBehaviour
{
    [SerializeField] private bool groundPoundPossible = true;
    [SerializeField] private float downwardsForce;
    [SerializeField] private Timer delay = new Timer(0.5f);
    private bool groundPound = false;

    // Update is called once per frame
    private void Update()
    {
        delay.countDown();
        if (UserInput.Input.GroundPound() && !delay.timerActive() && groundPoundPossible)
        {
            groundPound = true;
            groundPoundPossible = false;
            delay.resetTimer();
        }

        if (GetComponent<JumpBehavior>().IsGrounded)
        {
            groundPoundPossible = true;
            groundPound = false;
        }
    }

    private void FixedUpdate()
    {
        if (groundPound)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.down * downwardsForce);
        }
    }
}