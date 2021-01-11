using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JumpBehavior))]
public class FullStop : MonoBehaviour
{
    private bool fullStopPossible = true;
    [SerializeField] private Timer cooldown = new Timer(2);
    private bool fullStop = false;

    private void Update()
    {
        cooldown.CountDown();
        if (UserInput.Input.FullStop() && !cooldown.TimerActive() && fullStopPossible)
        {
            fullStop = true;
            fullStopPossible = false;
            cooldown.ResetTimer();
        }

        if (GetComponent<JumpBehavior>().IsGrounded)
        {
            fullStopPossible = true;
        }
    }

    private void FixedUpdate()
    {
        if (fullStop)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            fullStop = false;
        }
    }
}