using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionCheck))]
public class Friction : MonoBehaviour
{
    private CollisionCheck playerCollision;
    private Rigidbody rb;

    [SerializeField] private float airFriction = 0.01f;
    [SerializeField] private float kineticGroundFriction = 0.08f;

    private void Awake()
    {
        playerCollision = GetComponent<CollisionCheck>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        playerCollision.CollisionDataCollected += ApplyFriction;
    }

    private void OnDisable()
    {
        playerCollision.CollisionDataCollected -= ApplyFriction;
    }

    private void ApplyFriction()
    {
        float amount = playerCollision.IsGrounded.current ? kineticGroundFriction : airFriction;

        Vector3 force = (rb.velocity - playerCollision.ConnectionVelocity.current) * amount;

        if (playerCollision.IsGrounded.current)
        {
            force = playerCollision.ProjectOnContactPlane(force);
        }

        rb.AddForce(-force, ForceMode.VelocityChange);
    }
}