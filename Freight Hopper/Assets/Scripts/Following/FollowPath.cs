using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    private BezierPath path;
    private Vector3 targetPos;
    private Rigidbody rb;
    private float t = 0.0f;

    [SerializeField]
    private PathCreator pathCreator;

    [SerializeField]
    private float targetVelocity;

    [SerializeField]
    private float followDistance;
    /*
    [SerializeField]
    private PID.Data forwardPIDData;

    [SerializeField]
    private PID.Data rotatePIDDataY;

    [SerializeField]
    private PID.Data rotatePIDDataX;

    private PID forwardPID = new PID();
    private PID rotatePID_Y = new PID();
    private PID rotatePID_X = new PID();
    */

    [SerializeField]
    private PID.Data roll_PID_data;
    private PID roll_PID = new PID();

    [SerializeField]
    private PID.Data hover_PID_data;
    private PID hover_PID = new PID();




    private void Start()
    {
        rb = this.transform.GetComponent<Rigidbody>();
        path = pathCreator.path;
        Initialize_PIDs();
        /*
        ApplyVelocityChange(2.0f, 0.5f);
        forwardPID.Initialize(forwardPIDData);
        rotatePID_Y.Initialize(rotatePIDDataY);
        rotatePID_X.Initialize(rotatePIDDataX);
        */
    }

    private void Initialize_PIDs()
    {
        roll_PID.Initialize(roll_PID_data);
        hover_PID.Initialize(hover_PID_data);
    }

    private void FixedUpdate()
    {
        AdjustTarget();
        Follow2();
    }

    /*
    void Follow1()
    {
        //Transform matrices
        Matrix4x4 mat = Matrix4x4.TRS(this.transform.position, this.transform.rotation, Vector3.one);
        Matrix4x4 matDir = Matrix4x4.Rotate(this.transform.rotation);
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
    */

    void Follow2()
    {
        Debug.DrawLine(rb.position, targetPos);

        float roll_target = 0.0f;
        float roll_error = wrap_difference(roll_target - transform.eulerAngles.z, 360);
        float roll_torque = roll_PID.GetOutput(roll_error, Time.fixedDeltaTime);
        rb.AddRelativeTorque(Vector3.forward * roll_torque, ForceMode.Acceleration);

        hover_PID.Initialize(hover_PID_data);
        Matrix4x4 matY = Matrix4x4.TRS(this.transform.position, Quaternion.Euler(0,transform.eulerAngles.y,0), Vector3.one);
        Vector3 localDisplacement = matY.inverse.MultiplyPoint3x4(targetPos);
        float hover_error = localDisplacement.y;
        float hover_force = Mathf.Clamp(hover_PID.GetOutput(hover_error, Time.fixedDeltaTime), 0.0f, 75.0f);
        rb.AddForce(Vector3.up * hover_force, ForceMode.Acceleration);

        Debug.Log("Hover Error: " + hover_error);
        //Debug.Log("Displacement: " + localDisplacement);


    }

    float wrap_difference(float diff, float range)
    {
        return (Mathf.Abs(diff) <= range / 2.0f) ? diff : (diff - Mathf.Sign(diff) * range);
    }

    private void AdjustTarget() //TODO: If entire path is within follow distance, this will be a forever loop. Prevent this
    {
        Vector3 parentPos = new Vector3(0, 3, 0);
        while ((path.GetPathPoint(t) - this.transform.position).magnitude < followDistance)
        {
            t += 0.01f;
            if (t >= path.NumSegments)
            {
                break;
            }
        }
        targetPos = path.GetPathPoint(t) + parentPos;
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