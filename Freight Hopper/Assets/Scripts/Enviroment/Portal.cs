using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Portal otherPortal;
    [SerializeField] private Vector3 exitPosition;

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward);
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawDottedLine(this.transform.position, otherPortal.transform.position, 2);

        UnityEditor.Handles.DrawWireCube(otherPortal.transform.position + otherPortal.transform.TransformDirection(exitPosition), Vector3.one * 0.25f);
    }

    private void OnTriggerEnter(UnityEngine.Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.position = otherPortal.transform.position + otherPortal.transform.TransformDirection(exitPosition);
            Vector3 reflectionNormal = (this.transform.forward + otherPortal.transform.forward) * 0.5f;
            Debug.DrawRay((this.transform.position - otherPortal.transform.position), reflectionNormal, Color.red);
            Vector3 newVelocity = Vector3.Reflect(collision.attachedRigidbody.velocity, reflectionNormal);
            collision.attachedRigidbody.velocity = newVelocity;
        }
    }
}