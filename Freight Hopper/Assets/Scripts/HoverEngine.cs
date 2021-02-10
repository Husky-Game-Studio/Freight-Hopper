using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverEngine : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float maxDistance;
    [SerializeField] private Vector3 offset;

    [Tooltip("Normal of engine. Will be normalized")]
    [SerializeField] private Vector3 direction;

    private Ray ray;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float forceMultiplier;

    private void Awake()
    {
        ray = new Ray(transform.position + offset, direction);
    }

    private void OnDrawGizmosSelected()
    {
        ray = new Ray(transform.position + offset, direction);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * maxDistance);
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
        {
            Vector3 force = -ray.direction * (forceMultiplier / hit.distance);
            rb.AddForceAtPosition(force, ray.origin, ForceMode.Force);
            //Debug.Log(this.name + " - force: " + force);
        }
    }
}