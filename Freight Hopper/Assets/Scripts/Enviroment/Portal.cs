using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Portal otherPortal;

    private HashSet<Rigidbody> teleportingObjects = new HashSet<Rigidbody>();

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, transform.forward);
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawDottedLine(this.transform.position, otherPortal.transform.position, 2);
        UnityEditor.Handles.DrawWireCube(otherPortal.transform.position, Vector3.one * 0.25f);
    }

#endif

    /// <summary>
    /// Grabs transform of connected portal
    /// </summary>
    /// <returns></returns>
    public Transform OtherPortal()
    {
        return otherPortal.transform;
    }

    /// <summary>
    /// Takes in a ray and teleports it to the other portal.
    /// Needs point the portal gets hit on by the raw to teleport
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="hitPoint"></param>
    public void TeleportRay(ref Ray ray, Vector3 hitPoint)
    {
        ray.origin = otherPortal.transform.TransformPoint(transform.InverseTransformPoint(hitPoint));
        ray.direction = otherPortal.transform.TransformDirection(transform.InverseTransformDirection(-ray.direction));
    }

    public void AddTeleportingRigidbody(Rigidbody rb)
    {
        teleportingObjects.Add(rb);
    }

    private void OnTriggerExit(Collider other)
    {
        if (teleportingObjects.Contains(other.attachedRigidbody))
        {
            teleportingObjects.Remove(other.attachedRigidbody);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            if (other.GetComponent<CollisionManagement>() == null)
            {
                return;
            }
            if (teleportingObjects.Contains(other.attachedRigidbody))
            {
                return;
            }

            otherPortal.AddTeleportingRigidbody(other.attachedRigidbody);
            Teleport(other);
            RotateVelocity(other);
        }
    }

    private void Teleport(Collider other)
    {
        other.transform.position = otherPortal.transform.position;
    }

    private void RotateVelocity(Collider other)
    {
        Vector3 velocity = Vector3.zero;
        if (velocity == Vector3.zero)
        {
            velocity = other.GetComponent<CollisionManagement>().Velocity.old;
        }

        Vector3 reflectionNormal = (this.transform.forward + otherPortal.transform.forward).normalized;
        Vector3 newVelocity = Vector3.Reflect(velocity, reflectionNormal);
        other.attachedRigidbody.velocity = newVelocity;
    }
}