using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Marked for deletion
/// </summary>
public class RigidbodyController : MonoBehaviour
{
    [SerializeField] private PID rotation;
    [SerializeField] private PID speed;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3 facePoint;
    [SerializeField] private Transform front;
    [SerializeField] private float tolerance;
    [SerializeField] private float desiredSpeed;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move(facePoint);
        Debug.DrawLine(facePoint - Vector3.up, facePoint + Vector3.up, Color.yellow);
    }

    private void Move(Vector3 position)
    {
        //Debug.Log("Player forward transform: " + transform.forward);
        //Debug.Log("Weird inverse transform point: " + transform.InverseTransformPoint(position));
        float errorTorque = transform.InverseTransformPoint(position).x - transform.forward.x;
        Vector3 torque = Vector3.up * rotation.GetOutput(errorTorque, Time.fixedDeltaTime);
        rb.AddRelativeTorque(torque, ForceMode.Force);

        float currentSpeed = Vector3.ProjectOnPlane(rb.velocity, Vector3.up).magnitude;

        float errorSpeed = desiredSpeed - currentSpeed;
        if (errorSpeed > 0)
        {
            float forward = speed.GetOutput(errorSpeed, Time.fixedDeltaTime);
            Vector3 force = Vector3.forward * forward;
            rb.AddRelativeForce(force, ForceMode.Force);
        }
    }
}