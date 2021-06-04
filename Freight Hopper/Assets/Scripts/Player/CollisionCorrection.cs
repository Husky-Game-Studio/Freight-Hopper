using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsManager))]
public class CollisionCorrection : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float correctionHeight;
    [SerializeField] private float playerRadius;
    [SerializeField] private float radius = 0.4f;
    [SerializeField] private Vector3 point1;
    [SerializeField] private Vector3 point2;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnDrawGizmosSelected()
    {
        GizmosExtensions.DrawWireCapsule(transform.TransformPoint(point1), transform.TransformPoint(point2), playerRadius + radius);
    }

    private void FixedUpdate()
    {
        Vector3 start = transform.TransformPoint(new Vector3(0, correctionHeight, 0));
        Vector3 rbDirection = Vector3.ProjectOnPlane(rb.velocity * Time.fixedDeltaTime, GetComponent<PhysicsManager>().collisionManager.ValidUpAxis);
        Vector3 end = rbDirection + (rbDirection.normalized * playerRadius) + start;
        Debug.DrawLine(start, end, Color.white, Time.fixedDeltaTime);
    }
}