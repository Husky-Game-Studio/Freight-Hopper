using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistantRigidbodyLinker : MonoBehaviour
{
    [SerializeField] private Vector3 pointOne;
    [SerializeField] private Vector3 pointTwo;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask layerMask;
    private Rigidbody rb;
    private RigidbodyLinker rigibodyLinker;
    private CollisionManagement collisionManagent;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(this.transform.TransformPoint(pointOne), radius);
        Gizmos.DrawWireSphere(this.transform.TransformPoint(pointTwo), radius);
    }

    private void Awake()
    {
        rb = Player.Instance.modules.rigidbody;
        rigibodyLinker = Player.Instance.modules.rigidbodyLinker;
        collisionManagent = Player.Instance.modules.collisionManagement;
    }

    private void FixedUpdate()
    {
        EmitSkin();
    }

    private void EmitSkin()
    {
        // We only want to run this script if we are not connected to anything
        if (rigibodyLinker.ConnectedRb.old || collisionManagent.IsGrounded.old)
        {
            return;
        }

        Vector3 gravityDirection = -CustomGravity.GetUpAxis(rb.position);

        Collider[] colliders = Physics.OverlapCapsule(this.transform.TransformPoint(pointOne), this.transform.TransformPoint(pointTwo), radius, layerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].attachedRigidbody != null)
            {
                Vector3 dir = (colliders[i].attachedRigidbody.position - rb.position).normalized;
                //if (Vector3.Dot(dir, gravityDirection) < 0) // Only connect downwards
                //{
                Debug.Log("Updating link due to skin");
                rigibodyLinker.UpdateLink(colliders[i].attachedRigidbody);
                //}
            }
        }
    }
}