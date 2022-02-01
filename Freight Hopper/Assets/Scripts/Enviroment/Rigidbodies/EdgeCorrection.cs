using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCorrection : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float nextBoxVerticalDif;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Vector3 extents;

    private Rigidbody rb;
    private RigidbodyLinker rigibodyLinker;
    private CollisionManagement collisionManagent;

    private void Awake()
    {
        rb = Player.Instance.modules.rigidbody;
        rigibodyLinker = Player.Instance.modules.rigidbodyLinker;
        collisionManagent = Player.Instance.modules.collisionManagement;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position +
            (transform.right * offset.x) + (transform.up * offset.y) + (transform.forward * offset.z);
        Gizmos.DrawWireSphere(center, 1);
    }

    private void FixedUpdate()
    {
        CheckEdge();
    }

    private void CheckEdge()
    {
        bool bottomDetected;
        bool topDetected;
        Vector3 gravityDirection = CustomGravity.GetUpAxis(rb.position);
        Vector3 velocity = Vector3.ProjectOnPlane(rb.velocity, gravityDirection);
        Vector3 direction = velocity.normalized;
        float length = velocity.magnitude * Time.fixedDeltaTime;
        Vector3 center = rb.position +
            (rb.transform.right * offset.x) + (rb.transform.up * offset.y) + (rb.transform.forward * offset.z) +
            (direction * length / 2f);
        extents.z = length;
        ExtDebug.DrawBox(center, extents, Quaternion.LookRotation(rb.transform.forward, rb.transform.up), Color.red);
        ExtDebug.DrawBox(center + rb.transform.up * nextBoxVerticalDif, extents, Quaternion.LookRotation(rb.transform.forward, rb.transform.up), Color.red);
        bottomDetected = Physics.CheckBox(center, extents, Quaternion.LookRotation(rb.transform.forward, rb.transform.up), layerMask);
        topDetected = Physics.CheckBox(center + rb.transform.up * nextBoxVerticalDif, extents, Quaternion.LookRotation(rb.transform.forward, rb.transform.up), layerMask);

        if (bottomDetected && !topDetected)
        {
            rb.MovePosition(rb.position + rb.transform.up * nextBoxVerticalDif);
        }
    }
}