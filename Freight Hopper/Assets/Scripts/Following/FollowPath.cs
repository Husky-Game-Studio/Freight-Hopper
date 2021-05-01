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
    PID.Data rotatePIDDataY;
    [SerializeField]
    PID.Data rotatePIDDataX;

    PID forwardPID = new PID();
    PID rotatePID_Y = new PID();
    PID rotatePID_X = new PID();


    private void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        path = pathCreator.path;
        ApplyVelocityChange(2.0f, 0.5f);
        forwardPID.Initialize(forwardPIDData);
        rotatePID_Y.Initialize(rotatePIDDataY);
        rotatePID_X.Initialize(rotatePIDDataX);
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
        float turnRadius = deltaPosRelative.sqrMagnitude / (2.0f * new Vector2(deltaPosRelative.x, deltaPosRelative.y).magnitude);
        float targetAngularVelocity = targetVelocity / turnRadius;
        float turnXYAngle = Mathf.Atan2(deltaPosRelative.y, deltaPosRelative.x);
        float targetAngularVelocityY = targetAngularVelocity * Mathf.Cos(turnXYAngle);
        float targetAngularVelocityX = targetAngularVelocity * Mathf.Sin(turnXYAngle);
        //Calculate Acceleration to apply
        float forwardForce = forwardPID.GetOutput(targetVelocity - currentForwardVelocity, Time.fixedDeltaTime);
        float torqueY = rotatePID_Y.GetOutput(targetAngularVelocityY - rb.angularVelocity.y, Time.fixedDeltaTime);
        float torqueX = -rotatePID_X.GetOutput(targetAngularVelocityX - rb.angularVelocity.x, Time.fixedDeltaTime);
        //Apply Forces
        rb.AddRelativeForce(Vector3.forward * forwardForce, ForceMode.Acceleration);
        rb.AddRelativeTorque(Vector3.up * torqueY, ForceMode.Acceleration);
        rb.AddRelativeTorque(Vector3.right * torqueX, ForceMode.Acceleration);
        //Turning Constraint Force
        Vector3 radialVector = new Vector3(Mathf.Cos(turnXYAngle), Mathf.Sin(turnXYAngle), 0);
        rb.AddRelativeForce(radialVector * currentForwardVelocity * new Vector2(rb.angularVelocity.x, rb.angularVelocity.y).magnitude, ForceMode.Acceleration);
        //Debug Drawing
        Debug.DrawLine(rb.position, rb.position + 4.0f * (Vector3)(mat * radialVector));
        Debug.DrawLine(rb.position, targetPos);
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
