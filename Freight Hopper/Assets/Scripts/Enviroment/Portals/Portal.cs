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
        Gizmos.DrawRay(this.transform.position, this.transform.forward);
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
    /// Takes in a ray and teleports it to the other portal. Needs point the portal gets hit on by
    /// the raw to teleport
    /// </summary>
    public void TeleportRay(ref Ray ray, Vector3 hitPoint)
    {
        ray.origin = otherPortal.transform.TransformPoint(this.transform.InverseTransformPoint(hitPoint));
        ray.direction = otherPortal.transform.TransformDirection(this.transform.InverseTransformDirection(-ray.direction));
    }

    /// <summary>
    /// Adds rigidbody to teleporting objects set. This set is to indicate to the portal to not
    /// reteleport the object instantly
    /// </summary>
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
        RigidbodyReference rbReference = other.GetComponent<RigidbodyReference>();
        if (other.attachedRigidbody != null || rbReference != null)
        {
            Rigidbody rb = other.attachedRigidbody == null ? rbReference.reference : other.attachedRigidbody;
            if (rb == null)
            {
                return;
            }
            if (teleportingObjects.Contains(rb))
            {
                return;
            }

            otherPortal.AddTeleportingRigidbody(rb);
            Teleport(rb);
            RotateVelocity(rb);
        }
    }

    private void Teleport(Rigidbody other)
    {
        Quaternion oldRotation = this.transform.rotation;
        this.transform.rotation = Quaternion.FromToRotation(this.transform.forward, otherPortal.transform.forward) * this.transform.rotation;
        var transformMatrix = otherPortal.transform.localToWorldMatrix * this.transform.worldToLocalMatrix * other.transform.localToWorldMatrix;
        this.transform.rotation = oldRotation;

        other.position = transformMatrix.GetColumn(3);

        other.rotation = Quaternion.FromToRotation(-this.transform.forward, otherPortal.transform.forward) * other.rotation;
    }

    private void RotateVelocity(Rigidbody other)
    {
        Vector3 velocity = Vector3.zero;
        if (velocity == Vector3.zero)
        {
            //velocity = other.GetComponent<PhysicsManager>().collisionManager.Velocity.old;
        }
        // WARNING: THIS DOESN'T WORK ANYMORE AS ALL OBJECTS DON'T KEEP TRACK OF VELOCITY OLD

        Vector3 reflectionNormal = (this.transform.forward + otherPortal.transform.forward).normalized;
        Vector3 newVelocity = Vector3.Reflect(velocity, reflectionNormal);
        other.velocity = newVelocity;
    }
}