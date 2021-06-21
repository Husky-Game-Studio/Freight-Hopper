using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyPad : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float percentReflection = 1;

    //[SerializeField] private Vector3 normal;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(this.transform.position, this.transform.TransformDirection(Vector3.up) * percentReflection);
    }

    private void Awake()
    {
        this.GetComponent<BoxCollider>().material.bounciness = percentReflection;
    }

    /*private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Bounce(collision.collider);
        }
    }

    private void Bounce(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        Vector3 otherVelocity = rb.velocity;
        rb.velocity = Vector3.zero;
        Vector3 reflectDirection = Vector3.Reflect(otherVelocity, transform.TransformDirection(normal)) * percentReflection;
        Debug.Log("reflectDirection " + reflectDirection);
        rb.velocity = reflectDirection;
    }*/
}