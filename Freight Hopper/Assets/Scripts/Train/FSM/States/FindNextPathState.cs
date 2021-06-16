using System.Collections.Generic;
using System;
using UnityEngine;

public class FindNextPathState : BasicState
{
    private TrainMachineCenter trainMachineCenter;
    private Vector3 startPosition;

    public FindNextPathState(FiniteStateMachineCenter machineCenter, List<Func<BasicState>> stateTransitions) : base(machineCenter, stateTransitions)
    {
        this.trainMachineCenter = (TrainMachineCenter)machineCenter;
    }

    public override void EntryState()
    {
        trainMachineCenter.ChangePath();
        startPosition = trainMachineCenter.TargetPos(0);

        // Sparks fly
    }

    public override void ExitState()
    {
        // Sparks fly
    }

    public override void PerformBehavior()
    {
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();
        return state;
    }
}