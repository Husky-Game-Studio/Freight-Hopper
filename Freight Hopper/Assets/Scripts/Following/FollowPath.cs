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
        //Initialize_PIDs();
        //rb.velocity = Vector3.forward * targetVelocity;
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
        Debug.DrawLine(rb.position, targetPos);
        Follow();
    }

    

    void Follow()
    {
        Vector3 target = targetPos - rb.position;
        Quaternion rot = Quaternion.Inverse(transform.rotation);
        //Current
        Vector3 currentVel = rot * rb.velocity;
        Vector3 currentAngVel = rot * rb.angularVelocity;
        //Target
        Vector3 targetVel = Vector3.forward * targetVelocity;
        Vector3 targetAngVel = currentVel.z * TargetSphereDrive.TargetAngularVelocity(rot * target);
        //Change
        Vector3 deltaVel = targetVel - currentVel;
        Vector3 deltaAngVel = targetAngVel - currentAngVel;
        //Forces
        rb.AddRelativeForce(Vector3.forward * deltaVel.z, ForceMode.VelocityChange);
        rb.AddRelativeTorque(deltaAngVel, ForceMode.VelocityChange);
        rb.AddForce(TurningConstraint(rb.velocity, rb.angularVelocity), ForceMode.Acceleration);

    }

    //Testing PIDs
    void Follow2()
    {
        Debug.DrawLine(rb.position, targetPos);

        float roll_target = 0.0f;
        float roll_error = wrap_difference(roll_target - transform.eulerAngles.z, 360);
        float roll_torque = roll_PID.GetOutput(roll_error, Time.fixedDeltaTime);
        rb.AddRelativeTorque(Vector3.forward * roll_torque, ForceMode.Acceleration);

        hover_PID.Initialize(hover_PID_data);
        Matrix4x4 matY = Matrix4x4.TRS(this.transform.position, Quaternion.Euler(0, transform.eulerAngles.y, 0), Vector3.one);
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
        while ((path.GetPathPoint(t) + parentPos - this.transform.position).magnitude < followDistance)
        {
            t += 0.01f;
            if (t >= path.NumSegments)
            {
                break;
            }
        }
        targetPos = path.GetPathPoint(t) + parentPos;
    }

    private Vector3 TurningConstraint(Vector3 vel, Vector3 angVel)
    {
        return -Vector3.Cross(Vector3.ProjectOnPlane(vel, angVel), angVel);
    }
}