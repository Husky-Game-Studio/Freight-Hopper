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

        stepRayUpper.position = new Vector3(stepRayUpper.position.x, stepHeight + stepRayLower.position.y, stepRayUpper.position.z);
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
        float dist = rb.velocity.magnitude * Time.fixedDeltaTime * 1.1f;
        //Debug.DrawLine(stepRayLower.position, stepRayLower.position + Vector3.ProjectOnPlane(rb.velocity, rb.transform.up).normalized * dist);
        //Debug.DrawLine(stepRayUpper.transform.position + rb.transform.up * stepHeight, stepRayUpper.position + rb.transform.up * stepHeight + Vector3.ProjectOnPlane(rb.velocity, rb.transform.up).normalized * (dist + 0.1f));
        Vector3 velDir = Vector3.ProjectOnPlane(rb.velocity, rb.transform.up).normalized;
        Vector3[] direction =
        {
            velDir/*,
            Quaternion.AngleAxis(-15, rb.transform.up) * velDir,
            Quaternion.AngleAxis(15, rb.transform.up) * velDir*/
        };

        for (int i = 0; i < direction.Length; i++)
        {
            Debug.DrawLine(stepRayLower.position, stepRayLower.position + direction[i] * dist);
            Debug.DrawLine(stepRayUpper.position, stepRayUpper.position + direction[i] * (dist + 0.2f));

            if (Physics.Raycast(stepRayLower.position, direction[i], out RaycastHit hitLow, dist, layerMask))
            {
                if (Vector3.Angle(hitLow.normal, rb.transform.up) < 60)
                {
                    continue;
                }
                Vector3 tempDir = -rb.transform.up;
                Vector3 tempPos = hitLow.point + (tempDir * 0.1f) + rb.transform.up * stepHeight;
                Debug.DrawLine(tempPos, tempPos + tempDir * (dist + 0.2f), Color.red);
                if (Physics.Raycast(tempPos, tempDir, out RaycastHit hitDown, stepHeight, layerMask))
                {
                    rb.MovePosition(rb.position + hitDown.point - hitLow.point + 0.1f * rb.transform.up);
                }
                /*if (!Physics.Raycast(stepRayUpper.transform.position + rb.transform.up * stepHeight, direction[i], out _, dist + 0.2f, layerMask))
                {
                    rb.position -= new Vector3(0, -stepSmooth * Time.fixedDeltaTime, 0);
                }*/
            }
        }
    }
}