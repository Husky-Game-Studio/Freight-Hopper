using System;
using System.Collections.Generic;
using UnityEngine;

public class FollowPathState : BasicState
{
    private TrainMachineCenter trainFSM;
    private BezierPath path;
    private Vector3 targetPos;
    private bool endOfPath;
    public bool EndOfPath => endOfPath;
    public Vector3 TargetPos => targetPos;
    private float t = 0.0f;

    public FollowPathState(FiniteStateMachineCenter machineCenter, List<Func<BasicState>> stateTransitions) : base(machineCenter, stateTransitions)
    {
        this.trainFSM = (TrainMachineCenter)machineCenter;
    }

    public override void EntryState()
    {
        path = trainFSM.GetCurrentPath();
        t = 0.0f;
        endOfPath = false;
        // Sparks fly
    }

    public override void ExitState()
    {
        // Sparks fly
    }

    private void AdjustTarget()
    {
        while ((trainFSM.TargetPos(t) - trainFSM.rb[0].transform.position).magnitude < trainFSM.FollowDistance)
        {
            t += 0.01f;
            if (t >= path.NumSegments)
            {
                t = path.NumSegments;
                endOfPath = true;
                return;
            }
        }

        targetPos = trainFSM.TargetPos(t);
    }

    public override void PerformBehavior()
    {
        AdjustTarget();
        Debug.DrawLine(trainFSM.rb[0].position, targetPos);
        trainFSM.Follow(targetPos);
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();
        return state;
    }
}