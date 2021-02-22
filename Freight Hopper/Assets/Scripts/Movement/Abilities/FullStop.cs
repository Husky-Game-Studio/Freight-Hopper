using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionCheck))]
public class FullStop : MonoBehaviour
{
    private bool fullStopPossible = true;
    [SerializeField] private Timer cooldown = new Timer(2);
    private bool fullStop = false;
    private CollisionCheck playerCollision;
    private Rigidbody rigidbody1;

    private void Start()
    {
        // Gets the rigid body component and jump behaviour script
        rigidbody1 = GetComponent<Rigidbody>();
        playerCollision = GetComponent<CollisionCheck>();
    }

    private void Update()
    {
        // Starts the countdown
        cooldown.CountDown();
        // Checks if the placer can full stop
        if (UserInput.Input.FullStopTriggered() && !cooldown.TimerActive() && fullStopPossible)
        {
            fullStop = true;
            fullStopPossible = false;
            cooldown.ResetTimer();
        }
        // Checks if player is grounded if yes then its possible to full stop
        if (playerCollision.IsGrounded.current)
        {
            fullStopPossible = true;
        }
    }

    private void FixedUpdate()
    {
        // Stops the player
        if (fullStop)
        {
            rigidbody1.velocity = Vector3.zero;
            fullStop = false;
        }
    }
}