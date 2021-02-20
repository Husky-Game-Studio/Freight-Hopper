using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Realistically, this is not the final approach. This assuming too much about the gravity
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Gravity : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] public Var<float> scale = new Var<float>(1, 1);
    [SerializeField] private Vector3 direction = Vector3.up;
    public static readonly float constant = -9.81f;

    public Vector3 Direction => direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        scale.UpdateOld();
    }

    private float floatDelay;

    private void FixedUpdate()
    {
        // Is Sleeping is a physics engine thing. It basically stops physics interactions for an object if its not doing anything to save calculation cost
        if (rb.IsSleeping())
        {
            floatDelay = 0;
            return;
        }

        // This is temp until more advanced gravity and moving platforms. It basically waits a second tos ee if the player is not moving and if so stop adding force
        // Arbitrary small threshold. This is so that if the object is moving very slowly then no gravity is applied
        if (rb.velocity.sqrMagnitude < 0.0001f)
        {
            floatDelay += Time.deltaTime;
            if (floatDelay >= 1)
            {
                return;
            }
        }

        rb.AddForce(constant * direction * scale.current, ForceMode.Acceleration);
    }
}