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
        // Is Sleeping is a physics engine thing. It basically stops physics interactions for an object if its not doing anything to save calculation cost
        if (rb.IsSleeping())
        {
            floatDelay = 0;
            return;
        }

        // THIS WILL LIKELY CAUSE A FLOATING BUG UNTIL ITS UPDATE
        // This is temp until more advanced gravity and moving platforms. It basically waits a second to see if the player is not moving and if so stop adding force
        // Arbitrary small threshold. This is so that if the object is moving very slowly then no gravity is applied
        if (rb.velocity.sqrMagnitude < 0.00001f)
        {
            floatDelay += Time.deltaTime;
            if (floatDelay >= 5)
            {
                return;
            }
        }
        if (collid.IsGrounded.current)
        {
            return;
        }

        rb.AddForce(CustomGravity.GetGravity(rb.position), ForceMode.Acceleration);
    }
}