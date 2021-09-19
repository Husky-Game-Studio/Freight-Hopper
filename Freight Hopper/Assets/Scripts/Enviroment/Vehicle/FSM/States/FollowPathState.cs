using System;
using System.Collections.Generic;
using UnityEngine;

public class FollowPathState : BasicState
{
    private TrainMachineCenter trainFSM;
    private LinkedList<RailChangeMarker> railChangeMarkers = new LinkedList<RailChangeMarker>();
    private Vector3 targetDirection;

    public Vector3 TargetDirection => targetDirection;

    public FollowPathState(FiniteStateMachineCenter machineCenter, List<Func<BasicState>> stateTransitions) : base(machineCenter, stateTransitions)
    {
        this.trainFSM = (TrainMachineCenter)machineCenter;
    }

    public override void EntryState()
    {
        trainFSM.ChangePath();

        trainFSM.LinkTrainToPath(trainFSM.CurrentPath);

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
        TrainRailLinker.TrainData trainData;
        if (trainFSM.CurrentRailLinker == null)
        {
            return;
        }
        trainFSM.CurrentRailLinker.linkedRigidbodyObjects.TryGetValue(trainFSM.Locomotive.rb, out trainData);
        int index;
        if (trainData == null)
        {
            return;
        }
        else
        {
            index = Mathf.Min(trainData.followIndex, trainFSM.GetCurrentPath().path.localPoints.Length - 1);
        }

        //Debug.Log("follow index " + index);

        //Debug.Log("forward direction: " + trainFSM.GetCurrentPath().path.GetTangent(index));
        trainFSM.Follow(trainFSM.GetCurrentPath().path.GetTangent(index));

        if (trainFSM.GetNextRailLinker.WithinFollowDistance(0, trainFSM.Locomotive.rb.position)
            || trainFSM.CurrentRailLinker.WithinFollowDistance(trainFSM.CurrentRailLinker.pathCreator.path.localPoints.Length - 1, trainFSM.Locomotive.rb.position))
        {
            RailChangeMarker newMarker = new RailChangeMarker(trainFSM.carts, trainFSM.CurrentRailLinker, trainFSM.GetNextRailLinker);
            //Debug.Log("new rail change marker made");
            railChangeMarkers.AddLast(newMarker);
            trainFSM.ChangePath();
        }
        if (railChangeMarkers.Count < 1)
        {
            return;
        }
        foreach (RailChangeMarker marker in railChangeMarkers)
        {
            marker.UpdateMarker();
        }

        if (railChangeMarkers.First.Value.Completed)
        {
            railChangeMarkers.RemoveFirst();
        }
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();
        return state;
    }
}