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

    public FollowPathState(FiniteStateMachineCenter machineCenter, List<Func<BasicState>> stateTransitions) : base(machineCenter, stateTransitions)
    {
        this.trainFSM = (TrainMachineCenter)machineCenter;
    }

    public override void EntryState()
    {
        trainFSM.currentRailLinker = trainFSM.LinkTrainToPath(trainFSM.CurrentPath);

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
        if (trainFSM.currentRailLinker.IsRigidbodyLinked(trainFSM.Locomotive.rb))
        {
            int index = trainFSM.currentRailLinker.linkedRigidbodyObjects[trainFSM.Locomotive.rb].followIndex;

            trainFSM.Follow(trainFSM.GetCurrentPath().path.GetTangent(index));
        }
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();
        return state;
    }
}