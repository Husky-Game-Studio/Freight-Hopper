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
        targetPosition = trainFSM.GetStartOfCurrentPath();
    }

    public override void ExitState()
    {
    }

    public bool ReachedTarget()
    {
        return Vector3.Distance(trainFSM.rb[0].position, targetPosition) <= trainFSM.RailSnappingDistance;
    }

    public override void PerformBehavior()
    {
        trainFSM.Follow(targetPosition);
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();
        return state;
    }
}