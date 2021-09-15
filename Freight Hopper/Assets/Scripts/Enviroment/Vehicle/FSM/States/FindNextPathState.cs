using System.Collections.Generic;
using System;
using UnityEngine;

public class FindNextPathState : BasicState
{
    private TrainMachineCenter trainFSM;
    private Vector3 targetPosition;

    public FindNextPathState(FiniteStateMachineCenter machineCenter, List<Func<BasicState>> stateTransitions) : base(machineCenter, stateTransitions)
    {
        this.trainFSM = (TrainMachineCenter)machineCenter;
    }

    public override void EntryState()
    {
        trainFSM.ChangePath();
        targetPosition = trainFSM.GetClosestPointOnCurrentPath();
        if (trainFSM.InstantlyAccelerate && trainFSM.Starting)
        {
            trainFSM.SetToMaxSpeed();
        }
    }

    public override void ExitState()
    {
    }

    // Returns true if the locomotive has reached the target position, which is the start of current path
    public bool ReachedTarget()
    {
        return Vector3.Distance(trainFSM.Locomotive.rb.position, targetPosition) <= trainFSM.CurrentRailLinker.FollowDistance;
    }

    public override void PerformBehavior()
    {
        trainFSM.Follow((targetPosition - trainFSM.Locomotive.rb.position).normalized);
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();
        return state;
    }
}