using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionCheck), typeof(Gravity))]
public class GroundPound : MonoBehaviour
{
    [ReadOnly, SerializeField] private bool groundPoundPossible = true;
    [ReadOnly, SerializeField] private bool groundPounding = false;
    [SerializeField] private float initialBurstForce;
    [SerializeField] private float downwardsForce;
    [SerializeField] private float slopeDownForce;

    private CollisionCheck playerCollision;
    private Gravity gravity;
    private Rigidbody rb;

    private void OnEnable()
    {
        UserInput.GroundPoundInput += TryGroundPound;
    }

    private void OnDisable()
    {
        UserInput.GroundPoundInput -= TryGroundPound;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gravity = GetComponent<Gravity>();
        playerCollision = GetComponent<CollisionCheck>();
    }

    private void Update()
    {
        // Checks if ground pound is possible
        if (!UserInput.Input.GroundPoundTriggered())
        {
            groundPoundPossible = true;
            groundPounding = false;
        }
    }

    private void TryGroundPound()
    {
        if (groundPoundPossible)
        {
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
            rb.AddForce(-gravity.Direction * downwardsForce, ForceMode.VelocityChange);
            groundPounding = true;
            groundPoundPossible = false;
        }
    }

    private void FixedUpdate()
    {
        if (groundPounding)
        {
            Vector3 direction = -gravity.Direction;
            if (playerCollision.IsGrounded.old)
            {
                Vector3 acrossSlope = Vector3.Cross(gravity.Direction, playerCollision.ContactNormal.old);
                Vector3 downSlope = Vector3.Cross(acrossSlope, playerCollision.ContactNormal.old);
                direction = downSlope;
                direction *= slopeDownForce;
            }
            else
            {
                direction *= downwardsForce;
            }

            rb.AddForce(direction, ForceMode.Acceleration);
        }
    }
}