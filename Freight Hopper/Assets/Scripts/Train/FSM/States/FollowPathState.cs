using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FollowPathState : BasicState
{
    [SerializeField]
    private float targetVelocity;

    [SerializeField]
    private float followDistance;

    private TrainMachineCenter trainMachineCenter;
    private BezierPath path;
    private Vector3 targetPos;
    private float t = 0.0f;

    public FollowPathState(FiniteStateMachineCenter machineCenter, List<Func<BasicState>> stateTransitions) : base(machineCenter, stateTransitions)
    {
        this.trainMachineCenter = (TrainMachineCenter)machineCenter;
    }

    public override void EntryState()
    {
        path = trainMachineCenter.GetCurrentPath();
        // Sparks fly
    }

    public override void ExitState()
    {
        // Sparks fly
    }

    private void Follow()
    {
        Vector3 target = targetPos - trainMachineCenter.rb.position;
        Quaternion rot = trainMachineCenter.transform.rotation;
        Quaternion rotInv = Quaternion.Inverse(rot);
        Vector3 currentVelDir = (trainMachineCenter.rb.velocity.magnitude != 0) ? trainMachineCenter.rb.velocity.normalized : Vector3.forward;
        Quaternion rotVel = Quaternion.LookRotation(currentVelDir);
        Quaternion rotVelInv = Quaternion.Inverse(rotVel);
        Debug.DrawLine(trainMachineCenter.rb.position, trainMachineCenter.rb.position + currentVelDir * 20.0f, Color.magenta);

        //Current
        Vector3 currentVel = trainMachineCenter.rb.velocity;
        Vector3 currentAngVel = trainMachineCenter.rb.angularVelocity;

        //Target (Rotate, Move Forward)
        Vector3 targetVel = rot * Vector3.forward * targetVelocity;
        Vector3 targetAngVel = currentVel.magnitude * (rot * TargetAngVel(rotInv * target)); //Rotate based on rotation heading

        //Target Change
        Vector3 deltaVel = targetVel - currentVel;
        Vector3 deltaAngVel = targetAngVel - currentAngVel;
        //Forces
        trainMachineCenter.rb.AddForce(deltaVel / Time.fixedDeltaTime, ForceMode.Acceleration);
        trainMachineCenter.rb.AddTorque(deltaAngVel / Time.fixedDeltaTime, ForceMode.Acceleration);

        //Rolling Correction
        float z = trainMachineCenter.transform.eulerAngles.z;
        z -= (z > 180) ? 360 : 0;
        trainMachineCenter.rb.AddRelativeTorque(Vector3.forward * -0.05f * z / Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    private void AdjustTarget()
    {
        while ((trainMachineCenter.TargetPos(t) - trainMachineCenter.transform.position).magnitude < followDistance)
        {
            t += 0.01f;
            if (t >= path.NumSegments)
            {
                t = path.NumSegments;
                return;
            }
        }

        targetPos = trainMachineCenter.TargetPos(t);
    }

    public override void PerformBehavior()
    {
        AdjustTarget();
        Debug.DrawLine(trainMachineCenter.rb.position, targetPos);
        Follow();
    }

    private Vector3 TargetAngVel(Vector3 target)
    {
        return 2 * new Vector3(-target.y, target.x, 0) / target.sqrMagnitude;
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();
        return state;
    }
}