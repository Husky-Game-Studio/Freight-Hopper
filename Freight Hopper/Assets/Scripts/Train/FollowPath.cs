using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    private List<BezierPath> paths;
    private int currentPath = 0;
    private Vector3 targetPos;
    private Rigidbody rb;
    private float t = 0.0f;
    private Accelerometer accelerometer;

    [SerializeField]
    private FloatBounds vertical_bounds;

    [SerializeField]
    private FloatBounds forward_bounds;

    [SerializeField]
    private FloatBounds horizontal_bounds;

    [SerializeField]
    private Vector3 followOffset;

    [SerializeField]
    private bool disablePitch;

    [SerializeField]
    private List<GameObject> pathObjects;

    [SerializeField]
    private float targetVelocity;

    [SerializeField]
    private float followDistance;

    [SerializeField]
    private float derailDistance;

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
        rb.maxAngularVelocity = 50;
        paths = new List<BezierPath>();
        for (int i = 0; i < pathObjects.Count; i++)
        {
            paths.Add(pathObjects[i].GetComponent<PathCreator>().path);
        }
        accelerometer = GetComponent<Accelerometer>();
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
        if (currentPath < paths.Count)
        {
            AdjustTarget();
            Debug.DrawLine(rb.position, targetPos);
            Follow6();
        }
    }

    private void Follow()
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
        float z = transform.eulerAngles.z;
        z -= (z > 180) ? 360 : 0;
        rb.AddRelativeTorque(Vector3.forward * -0.05f * z, ForceMode.VelocityChange);
        rb.AddForce(TurningConstraint(rb.velocity, rb.angularVelocity), ForceMode.Acceleration);
    }

    private void Follow3()
    {
        Vector3 target = targetPos - rb.position;
        Quaternion rot = Quaternion.Inverse(transform.rotation);
        //Current
        Vector3 currentVel = rot * rb.velocity;
        Vector3 currentAngVel = rot * rb.angularVelocity;
        //Target
        Vector3 targetVel = Vector3.forward * targetVelocity;
        Vector3 targetAngVel = currentVel.z * TargetSphereDrive.TargetAngularVelocity(rot * target);
        //Target Change
        Vector3 deltaVel = targetVel - currentVel;
        Vector3 deltaAngVel = targetAngVel - currentAngVel;
        //if (disablePitch)
        //{
        //    deltaVel = new Vector3(0.0f, deltaAng)
        //}
        deltaAngVel = new Vector3((disablePitch) ? 0.0f : deltaAngVel.x, deltaAngVel.y, deltaAngVel.z);

        //Doable Change
        //deltaVel = new Vector3(horizontal.Clamp(deltaVel.x), vertical.Clamp(deltaVel.y), forward.Clamp(deltaVel.z));
        //Too tight of turn to make? Slow down
        //Forces
        rb.AddRelativeForce(Vector3.forward * deltaVel.z / Time.fixedDeltaTime, ForceMode.Acceleration);
        rb.AddRelativeTorque(deltaAngVel / Time.fixedDeltaTime, ForceMode.Acceleration);
        //Rolling Correction
        float z = transform.eulerAngles.z;
        z -= (z > 180) ? 360 : 0;
        rb.AddRelativeTorque(Vector3.forward * -0.05f * z / Time.fixedDeltaTime, ForceMode.Acceleration);
        //Turning Constraint
        rb.AddForce(TurningConstraint(rb.velocity, rb.angularVelocity), ForceMode.Acceleration);
    }

    private void Follow4()
    {
        Vector3 target = targetPos - TargetPos(lastT);
        Debug.DrawLine(TargetPos(lastT), targetPos);
        Quaternion rot = Quaternion.Inverse(transform.rotation);
        //Current
        Vector3 currentVel = rb.velocity;
        Vector3 currentAngVel = rb.angularVelocity;

        //Target
        Vector3 targetVel = (target.normalized) * targetVelocity;

        // Vector3 targetAngVel = currentVel.z * TargetSphereDrive.TargetAngularVelocity(rot * target);
        Vector3 forward = target.normalized;
        Vector3 right = Vector3.Cross(GetComponent<PhysicsManager>().collisionManager.ValidUpAxis, target.normalized);
        Vector3 up = Vector3.Cross(target.normalized, right);
        Debug.DrawLine(transform.position, transform.position + forward, Color.blue, Time.fixedDeltaTime);
        Debug.DrawLine(transform.position, transform.position + right, Color.red, Time.fixedDeltaTime);
        Debug.DrawLine(transform.position, transform.position + up, Color.green, Time.fixedDeltaTime);

        Vector3 targetAngVel = TargetAngVel4(Quaternion.LookRotation(target.normalized, up) * rot);

        // Vector3 targetAngVel = currentVel * (rot * target);

        //Target Change
        Vector3 deltaVel = targetVel - currentVel;
        Vector3 deltaAngVel = targetAngVel - currentAngVel;
        //deltaAngVel = new Vector3((disablePitch) ? 0.0f : deltaAngVel.x, deltaAngVel.y, deltaAngVel.z);

        //Doable Change
        deltaVel = new Vector3(horizontal_bounds.Clamp(deltaVel.x), vertical_bounds.Clamp(deltaVel.y), forward_bounds.Clamp(deltaVel.z));
        //Too tight of turn to make? Slow down
        //Forces
        rb.AddForce(Vector3.ProjectOnPlane(deltaVel, GetComponent<PhysicsManager>().collisionManager.ValidUpAxis) / Time.fixedDeltaTime, ForceMode.Acceleration);
        rb.AddTorque(deltaAngVel / Time.fixedDeltaTime, ForceMode.Acceleration);
        //Rolling Correction
        //float z = transform.eulerAngles.z;
        //z -= (z > 180) ? 360 : 0;
        //Turning Constraint
        //rb.AddForce(TurningConstraint(rb.velocity, rb.angularVelocity), ForceMode.Acceleration);
    }

    private void Follow5()
    {
        Vector3 target = targetPos - rb.position;
        Quaternion rot = Quaternion.Inverse(transform.rotation);
        //Quaternion rot = Quaternion.Inverse(Quaternion.LookRotation(rb.velocity));
        //Current
        Vector3 currentVel = rot * rb.velocity;
        Vector3 currentAngVel = rot * rb.angularVelocity;
        //Target
        Vector3 targetVel = Vector3.forward * targetVelocity;
        Vector3 targetAngVel = currentVel.magnitude * TargetAngVel(rot * target);
        //targetVel += TurningConstraint(targetVel, targetAngVel);
        //Target Change
        Vector3 deltaVel = targetVel - currentVel;
        Vector3 deltaAngVel = targetAngVel - currentAngVel;
        //Doable Change
        deltaVel = new Vector3(horizontal_bounds.Clamp(deltaVel.x), vertical_bounds.Clamp(deltaVel.y), forward_bounds.Clamp(deltaVel.z));
        //Forces
        rb.AddRelativeForce(deltaVel / Time.fixedDeltaTime, ForceMode.Acceleration);
        rb.AddRelativeTorque(deltaAngVel / Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    private void Follow6()
    {
        Vector3 target = targetPos - rb.position;
        Quaternion rot = transform.rotation;
        Quaternion rotInv = Quaternion.Inverse(rot);
        Vector3 currentVelDir = (rb.velocity.magnitude != 0) ? rb.velocity.normalized : Vector3.forward;
        Quaternion rotVel = Quaternion.LookRotation(currentVelDir);
        Quaternion rotVelInv = Quaternion.Inverse(rotVel);
        Debug.DrawLine(rb.position, rb.position + currentVelDir * 20.0f, Color.magenta);
        //Current
        Vector3 currentVel = rb.velocity;
        Vector3 currentAngVel = rb.angularVelocity;

        //Target (Rotate, Move Forward)
        Vector3 targetVel = rot * Vector3.forward * targetVelocity;
        Vector3 targetAngVel = currentVel.magnitude * (rot * TargetAngVel(rotInv * target)); //Rotate based on rotation heading
        //Vector3 targetAngVel = currentVel.magnitude * (rotVel * TargetAngVel(rotVelInv * target)); //Rotate based on translation heading

        //Target (Translation, using no rotation data)
        //Vector3 targetVel = currentVelDir * targetVelocity;
        //targetVel += TurningConstraint(targetVel, rotVel * (TargetAngVel(rotVelInv * target)));
        //Vector3 targetAngVel = Vector3.zero;
        //Vector3 targetAngVel = currentVel.magnitude * (rotVel * TargetAngVel(rotVelInv * target));

        //Target Change
        Vector3 deltaVel = targetVel - currentVel;
        Vector3 deltaAngVel = targetAngVel - currentAngVel;
        //Forces
        rb.AddForce(deltaVel / Time.fixedDeltaTime, ForceMode.Acceleration);
        rb.AddTorque(deltaAngVel / Time.fixedDeltaTime, ForceMode.Acceleration);

        //Rolling Correction
        float z = transform.eulerAngles.z;
        z -= (z > 180) ? 360 : 0;
        rb.AddRelativeTorque(Vector3.forward * -0.05f * z / Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    //Testing PIDs
    private void Follow2()
    {
        Debug.DrawLine(rb.position, targetPos);

        float roll_target = 0.0f;
        float roll_error = WrapDifference(roll_target - transform.eulerAngles.z, 360);
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

    private float WrapDifference(float diff, float range)
    {
        return (Mathf.Abs(diff) <= range / 2.0f) ? diff : (diff - Mathf.Sign(diff) * range);
    }

    private float lastT;

    private void AdjustTarget()
    {
        if ((TargetPos(t) - this.transform.position).magnitude < followDistance)
        {
            lastT = t;
        }
        while ((TargetPos(t) - this.transform.position).magnitude < followDistance)
        {
            t += 0.01f;
            if (t >= paths[currentPath].NumSegments)
            {
                t = paths[currentPath].NumSegments;
                EndPath();
                return;
            }
        }
        if (IsDerail())
        {
            Derail();
            return;
        }
        targetPos = TargetPos(t);
    }

    private void EndPath()
    {
        // STOP FOLLOWING THE PATH
        currentPath++;
    }

    //
    private bool IsDerail()
    {
        float distanceFromCurve = (TargetPos(t) - this.transform.position).magnitude;
        if (distanceFromCurve > derailDistance)
        {
            return true;
        }
        return false;
    }

    //
    private void Derail()
    {
        currentPath = paths[currentPath].NumSegments;
    }

    private Vector3 TargetPos(float t)
    {
        return pathObjects[currentPath].transform.TransformPoint(paths[currentPath].GetPathPoint(t)) + followOffset;
    }

    private Vector3 TurningConstraint(Vector3 vel, Vector3 angVel)
    {
        return -Vector3.Cross(Vector3.ProjectOnPlane(vel, angVel), angVel);
    }

    private Vector3 TargetAngVel(Vector3 target)
    {
        return 2 * new Vector3(-target.y, target.x, 0) / target.sqrMagnitude;
    }

    private Vector3 TargetAngVel4(Quaternion targetDeltaRot)
    {
        Vector3 axis;
        float angle;
        targetDeltaRot.ToAngleAxis(out angle, out axis);
        angle -= (angle > 180) ? 360 : 0;
        return Mathf.Deg2Rad * angle * axis.normalized;
    }
}