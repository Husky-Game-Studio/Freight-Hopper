using System;
using System.Collections.Generic;
using UnityEngine;

public class FollowPathState : BasicState
{
    private TrainMachineCenter trainFSM;
    private PathCreation.PathCreator pathCreator;
    private TrainRailLinker railLinker;
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
        pathCreator = trainFSM.GetCurrentPathObject();
        railLinker = pathCreator.GetComponent<TrainRailLinker>();

        foreach (Cart cart in trainFSM.carts)
        {
            float tValueForCart = trainFSM.GetClosestTValueOnCurrentPath(cart.rb.position);
            railLinker.Link(cart.rb, tValueForCart);
        }

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
        targetDirection = pathCreator.path.GetDirection(pathCreator.path.GetClosestTimeOnPath(trainFSM.Locomotive.rb.transform.position));
        trainFSM.Follow(targetDirection.normalized);
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();
        return state;
    }
}