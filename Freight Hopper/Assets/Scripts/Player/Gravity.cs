using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Realistically, this is not the final approach. This assuming too much about the gravity
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(CollisionManagement))]
public class Gravity : MonoBehaviour
{
    private Rigidbody rb;
    private CollisionManagement collid;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        collid = GetComponent<CollisionManagement>();
    }

    private float floatDelay;

    public void OnEnable()
    {
        collid.CollisionDataCollected += ApplyGravity;
    }

    public void OnDisable()
    {
        collid.CollisionDataCollected -= ApplyGravity;
    }

    private void ApplyGravity()
    {
        if (collid.IsGrounded.current)
        {
            return;
        }

        rb.AddForce(CustomGravity.GetGravity(rb.position), ForceMode.Acceleration);
    }
}