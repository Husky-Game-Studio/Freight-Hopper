using System;
using System.Collections.Generic;
using UnityEngine;

public class FollowPathState : BasicState
{
    private TrainMachineCenter trainFSM;
    private BezierPath path;
    private Vector3 targetPos;
    private float t = 0.0f;

    public FollowPathState(FiniteStateMachineCenter machineCenter, List<Func<BasicState>> stateTransitions) : base(machineCenter, stateTransitions)
    {
        this.trainFSM = (TrainMachineCenter)machineCenter;
    }

    public override void EntryState()
    {
        path = trainFSM.GetCurrentPath();
        // Sparks fly
    }

    public override void ExitState()
    {
        // Sparks fly
    }

    private void Follow()
    {
        if(!trainFSM.hoverController.EnginesFiring) {
            return;
        }

        Vector3 target = targetPos - trainFSM.rb.position;
        Quaternion rot = trainFSM.transform.rotation;
        Quaternion rotInv = Quaternion.Inverse(rot);
        Vector3 currentVelDir = (trainFSM.rb.velocity.magnitude != 0) ? trainFSM.rb.velocity.normalized : Vector3.forward;
        Quaternion rotVel = Quaternion.LookRotation(currentVelDir);
        Quaternion rotVelInv = Quaternion.Inverse(rotVel);
        Debug.DrawLine(trainFSM.rb.position, trainFSM.rb.position + currentVelDir * 20.0f, Color.magenta);

        //Current
        Vector3 currentVel = trainFSM.rb.velocity;
        Vector3 currentAngVel = trainFSM.rb.angularVelocity;

        //Target (Rotate, Move Forward)
        Vector3 targetVel = rot * Vector3.forward * trainFSM.TargetVelocity;
        Vector3 targetAngVel = currentVel.magnitude * (rot * TargetAngVel(rotInv * target)); //Rotate based on rotation heading

        //Target Change
        Vector3 deltaVel = targetVel - currentVel;
        Vector3 deltaAngVel = targetAngVel - currentAngVel;

        //Target Acceleration
        Vector3 acc = deltaVel / Time.fixedDeltaTime;
        Vector3 angAcc = deltaAngVel / Time.fixedDeltaTime;
        
        //Limit Change
        acc = rot * (rotInv * acc).ClampComponents(new Vector3(-trainFSM.ForceBounds.x,0,0), trainFSM.ForceBounds);
        angAcc = rot * (rotInv * angAcc).ClampComponents(-trainFSM.TorqueBounds, trainFSM.TorqueBounds);

        //Forces
        trainFSM.rb.AddForce(acc, ForceMode.Acceleration);
        trainFSM.rb.AddTorque(angAcc, ForceMode.Acceleration);

        //Rolling Correction
        float z = trainFSM.transform.eulerAngles.z;
        z -= (z > 180) ? 360 : 0;
        trainFSM.rb.AddRelativeTorque(Vector3.forward * -0.05f * z / Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    private void AdjustTarget()
    {
        while ((trainFSM.TargetPos(t) - trainFSM.transform.position).magnitude < trainFSM.FollowDistance)
        {
            t += 0.01f;
            if (t >= path.NumSegments)
            {
                t = path.NumSegments;
                return;
            }
        }

        targetPos = trainFSM.TargetPos(t);
    }

    public override void PerformBehavior()
    {
        AdjustTarget();
        Debug.DrawLine(trainFSM.rb.position, targetPos);
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