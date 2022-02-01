using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCorrection : MonoBehaviour
{
    [SerializeField] private Transform stepRayUpper;
    [SerializeField] private Transform stepRayLower;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float stepHeight = 0.3f;
    [SerializeField] private float stepSmooth = 2;

    private Rigidbody rb;
    private RigidbodyLinker rigibodyLinker;
    private CollisionManagement collisionManagent;

    private void Awake()
    {
        rb = Player.Instance.modules.rigidbody;
        rigibodyLinker = Player.Instance.modules.rigidbodyLinker;
        collisionManagent = Player.Instance.modules.collisionManagement;

        stepRayUpper.position = new Vector3(stepRayUpper.position.x, stepHeight, stepRayUpper.position.z);
    }

    private void OnDrawGizmosSelected()
    {
    }

    private void FixedUpdate()
    {
        StepUp();
    }

    private void StepUp()
    {
        float dist = rb.velocity.magnitude * Time.fixedDeltaTime * 1.3f;
        //Debug.DrawLine(stepRayLower.position, stepRayLower.position + Vector3.ProjectOnPlane(rb.velocity, rb.transform.up).normalized * dist);
        //Debug.DrawLine(stepRayUpper.transform.position + rb.transform.up * stepHeight, stepRayUpper.position + rb.transform.up * stepHeight + Vector3.ProjectOnPlane(rb.velocity, rb.transform.up).normalized * (dist + 0.1f));
        if (Physics.Raycast(stepRayLower.position, Vector3.ProjectOnPlane(rb.velocity, rb.transform.up), out RaycastHit hitLow, dist, layerMask))
        {
            if (Vector3.Angle(hitLow.normal, rb.transform.up) < 60)
            {
                return;
            }
            if (!Physics.Raycast(stepRayUpper.transform.position + rb.transform.up * stepHeight, Vector3.ProjectOnPlane(rb.velocity, rb.transform.up), out _, dist + 0.5f, layerMask))
            {
                rb.position -= new Vector3(0, -stepSmooth * Time.fixedDeltaTime, 0);
                //Debug.Log("I stepped up!");
            }
        }
    }
}