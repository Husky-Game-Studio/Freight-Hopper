using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class GravityWell : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private float coreStrength;
    private List<Rigidbody> effectedBodies = new List<Rigidbody>();

    private void Awake()
    {
        GetComponent<SphereCollider>().radius = radius;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (Rigidbody rb in effectedBodies)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(rb.position, Vector3.Normalize(this.transform.position - rb.position));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            effectedBodies.Add(other.attachedRigidbody);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            effectedBodies.Remove(other.attachedRigidbody);
        }
    }

    private void FixedUpdate()
    {
        foreach (Rigidbody rb in effectedBodies)
        {
            rb.AddForce(Vector3.Normalize(this.transform.position - rb.position) * CalculateStrength(rb.position), ForceMode.Acceleration);
        }
    }

    private float CalculateStrength(Vector3 objectPosition)
    {
        float dist = Vector3.Distance(this.transform.position, objectPosition);
        return (radius - dist) / radius * coreStrength;
    }
}