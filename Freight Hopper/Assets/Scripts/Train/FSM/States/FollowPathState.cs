using System;
using System.Collections.Generic;
using UnityEngine;

public class FollowPathState : BasicState
{
    private TrainMachineCenter trainFSM;

    private Vector3 targetDirection;
    private bool endOfPath;
    public bool EndOfPath => endOfPath;
    public Vector3 TargetDirection => targetDirection;
    private TrainRailLinker currentLinker;

    public FollowPathState(FiniteStateMachineCenter machineCenter, List<Func<BasicState>> stateTransitions) : base(machineCenter, stateTransitions)
    {
        this.trainFSM = (TrainMachineCenter)machineCenter;
    }

    public override void EntryState()
    {
        currentLinker = trainFSM.LinkTrainToPath(trainFSM.CurrentPath);

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

    public override void PerformBehavior()
    {
        int index = currentLinker.linkedRigidbodyObjects[trainFSM.Locomotive.rb].followIndex;

        trainFSM.Follow(trainFSM.GetCurrentPath().path.GetTangent(index));
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();
        return state;
    }
}