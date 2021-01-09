using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BouncyPad : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float percentReflection = 1;

    [SerializeField] private Vector3 normal;

    private BoxCollider triggerCollider;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, normal);
    }

    private void Awake()
    {
        triggerCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Bounce(other);
        }
    }

    private void Bounce(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        Vector3 otherVelocity = rb.velocity;
        Vector3 reflectDirection = Vector3.Reflect(otherVelocity, normal) * percentReflection;
        Debug.Log("reflectDirection " + reflectDirection);
        rb.velocity = reflectDirection;
    }
}