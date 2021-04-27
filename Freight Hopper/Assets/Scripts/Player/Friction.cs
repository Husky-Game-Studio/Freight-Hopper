using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionManagement))]
public class Friction : MonoBehaviour
{
    private CollisionManagement playerCollision;
    private Rigidbody rb;

    [SerializeField] private float airFriction = 0.01f;
    [SerializeField] private float kineticGroundFriction = 0.08f;

    private void Awake()
    {
        playerCollision = GetComponent<CollisionManagement>();
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

        Vector3 force = (rb.velocity - playerCollision.rigidbodyLinker.ConnectionVelocity.current) * amount;

        if (playerCollision.IsGrounded.current)
        {
            force = force.ProjectOnContactPlane(playerCollision.ContactNormal.current);
        }

        rb.AddForce(-force, ForceMode.VelocityChange);
    }
}