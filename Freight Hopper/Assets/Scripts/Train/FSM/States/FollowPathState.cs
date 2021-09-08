using System;
using System.Collections.Generic;
using UnityEngine;

public class FollowPathState : BasicState
{
    private TrainMachineCenter trainFSM;
    private PathCreation.PathCreator pathCreator;
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
        pathCreator = trainFSM.GetCurrentPathObject();
        railLinker = pathCreator.GetComponent<TrainRailLinker>();

        foreach (Cart cart in trainFSM.carts)
        {
            float tValueForCart = trainFSM.GetClosestTValueOnCurrentPath(cart.rb.position);
            railLinker.Link(cart.rb, tValueForCart);
        }
        t = trainFSM.GetClosestTValueOnCurrentPath(trainFSM.carts.First.Value.rb.position);
        endOfPath = false;
        if (trainFSM.InstantlyAccelerate && trainFSM.Starting)
        {
            trainFSM.SetToMaxSpeed();
        }

        // Sparks fly
    }

    public override void ExitState()
    {
        // Sparks fly
    }

    private void AdjustTarget()
    {
        while ((trainFSM.currentRailLinker.TargetPos(t) - trainFSM.carts.First.Value.rb.transform.position).magnitude < trainFSM.currentRailLinker.FollowDistance)
        {
            t += 0.01f;
            if (t >= pathCreator.bezierPath.NumSegments)
            {
                t = pathCreator.bezierPath.NumSegments;
                endOfPath = true;
                return;
            }
        }

        targetPos = trainFSM.currentRailLinker.TargetPos(t);
    }

    public override void PerformBehavior()
    {
        AdjustTarget();
        //Debug.DrawLine(trainFSM.carts.First.Value.rb.position, targetPos);
        trainFSM.Follow(targetPos);
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();
        return state;
    }
}