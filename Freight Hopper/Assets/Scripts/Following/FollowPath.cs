using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    BezierPath path;
    Vector3 targetPos;
    Rigidbody rb;
    float t = 0.0f;

    [SerializeField]
    PathCreator pathCreator;
    [SerializeField]
    float targetVelocity;
    [SerializeField]
    float followDistance;
    [SerializeField]
    PID.Data forwardPIDData;
    [SerializeField]
    PID.Data rotatePIDData;

    PID forwardPID = new PID();
    PID rotatePID = new PID();
    

    private void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        path = pathCreator.path;
        ApplyVelocityChange(2.0f, 0.5f);
        forwardPID.Initialize(forwardPIDData);
        rotatePID.Initialize(rotatePIDData);
    }

    private void FixedUpdate()
    {
        AdjustTarget();
        //Transform matrices
        Matrix4x4 mat = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Matrix4x4 matDir = Matrix4x4.Rotate(transform.rotation);
        //Transformed variables
        Vector3 deltaPosRelative = mat.inverse.MultiplyPoint3x4(targetPos);
        float currentForwardVelocity = (matDir.inverse * rb.velocity).z;
        //Turning calculations
        float turnRadius = deltaPosRelative.sqrMagnitude / (2.0f * deltaPosRelative.x);
        float targetAngularVelocity = targetVelocity / turnRadius;
        //Apply forces
        ApplyVelocityChange(targetVelocity - currentForwardVelocity, targetAngularVelocity - rb.angularVelocity.y);
        
        //float forwardForce = forwardPID.GetOutput(targetVelocity - currentForwardVelocity, Time.fixedDeltaTime);
        //float torque = rotatePID.GetOutput(targetAngularVelocity - rb.angularVelocity.y, Time.fixedDeltaTime);
        //ApplyForce(forwardForce, torque);

        TurningConstraintForce(currentForwardVelocity, rb.angularVelocity.y);
    }

    private void AdjustTarget() //TODO: If entire path is within follow distance, this will be a forever loop. Prevent this
    {
        while ((path.GetPathPoint(t) - transform.position).magnitude < followDistance)
        {
            t += 0.01f;
            if (t >= path.NumSegments)
            {
                break;
            }
                
        }
        targetPos = path.GetPathPoint(t);
    }

    private void ApplyVelocityChange(float forward, float turn)
    {
        rb.AddRelativeForce(Vector3.forward * forward, ForceMode.VelocityChange);
        rb.AddRelativeTorque(Vector3.up * turn, ForceMode.VelocityChange);
    }

    private void ApplyForce(float forward, float turn)
    {
        rb.AddRelativeForce(Vector3.forward * forward, ForceMode.Acceleration);
        rb.AddRelativeTorque(Vector3.up * turn, ForceMode.Acceleration);
    }

    private void TurningConstraintForce(float currentForwardVelocity, float deltaTheta)
    {
        rb.AddRelativeForce(Vector3.right * currentForwardVelocity * deltaTheta, ForceMode.Acceleration);
    }
}
