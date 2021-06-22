using System;
using System.Collections.Generic;
using UnityEngine;

public class FollowPathState : BasicState
{
    private TrainMachineCenter trainFSM;
    private PathCreator pathCreator;
    private TrainRailLinker railLinker;
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
        pathCreator = trainFSM.GetCurrentPathObject().pathCreator;
        railLinker = pathCreator.GetComponent<TrainRailLinker>();

        for (int i = 0; i < trainFSM.cartRigidbodies.Length; i++)
        {
            railLinker.Link(trainFSM.cartRigidbodies[i]);
        }
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
        while ((trainFSM.currentRailLinker.TargetPos(t) - trainFSM.cartRigidbodies[0].transform.position).magnitude < trainFSM.currentRailLinker.FollowDistance)
        {
            t += 0.01f;
            if (t >= pathCreator.path.NumSegments)
            {
                t = pathCreator.path.NumSegments;
                endOfPath = true;
                return;
            }
        }

        targetPos = trainFSM.currentRailLinker.TargetPos(t);
    }

    public override void PerformBehavior()
    {
        AdjustTarget();
        Debug.DrawLine(trainFSM.cartRigidbodies[0].position, targetPos);
        trainFSM.Follow(targetPos);
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();
        return state;
    }
}