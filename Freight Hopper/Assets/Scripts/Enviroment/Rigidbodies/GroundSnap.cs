using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Snaps given rigidbody to the ground. In this case only the player
public class GroundSnap : MonoBehaviour
{
    [SerializeField] private float verticalOffset;
    [SerializeField, Min(0.0001f)] private float rayDistance;
    [SerializeField] private LayerMask layerMask;
    // This is the angle from the down direction and the velocity direction at which we snap E.g.,
    // if that angle is 100 degrees or less we snap the player to the ground. Otherwise we don't
    [SerializeField] private float snappingAngle;
    private Rigidbody rb;
    private RigidbodyLinker rigibodyLinker;
    private CollisionManagement collisionManagent;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(this.transform.position + Vector3.up * verticalOffset, this.transform.position + Vector3.up * verticalOffset + (Vector3.down * rayDistance));
    }

    private void Awake()
    {
        rb = Player.Instance.modules.rigidbody;
        rigibodyLinker = Player.Instance.modules.rigidbodyLinker;
        collisionManagent = Player.Instance.modules.collisionManagement;
    }

    private void FixedUpdate()
    {
        EmitRay();
    }

    private void EmitRay()
    {
        // We only want to run this script if we are not connected to anything
        if (rigibodyLinker.ConnectedRb.old || collisionManagent.IsGrounded.old)
        {
            return;
        }

        Vector3 gravityDirection = -CustomGravity.GetUpAxis();

        // player is going up don't check for snap
        //Debug.Log(Mathf.Acos(Vector3.Dot((rb.velocity - rigibodyLinker.ConnectionVelocity.old).normalized, gravityDirection)) * Mathf.Rad2Deg);
        if (Mathf.Acos(Vector3.Dot((rb.velocity - rigibodyLinker.ConnectionVelocity.old).normalized, gravityDirection)) * Mathf.Rad2Deg > snappingAngle)
        {
            return;
        }

        Ray ray = new Ray(rb.position + verticalOffset * this.transform.up, gravityDirection);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, layerMask))
        {
            float dist = hit.distance;
            rb.MovePosition(rb.position + ray.direction * dist);
            //Debug.Log("snapping");
        }
    }
}