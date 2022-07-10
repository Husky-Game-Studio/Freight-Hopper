using System;
using System.Collections.Generic;

public class FollowPathState : BasicState
{
    private TrainMachineCenter trainFSM;
    private LinkedList<RailChangeMarker> railChangeMarkers = new LinkedList<RailChangeMarker>();
    private bool dummyTrain;

    public FollowPathState(FiniteStateMachineCenter machineCenter, List<Func<BasicState>> stateTransitions, bool dummyTrain) : base(machineCenter, stateTransitions)
    {
        this.trainFSM = (TrainMachineCenter)machineCenter;
        this.dummyTrain = dummyTrain;
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
        if (dummyTrain)
        {
            trainFSM.Follow(trainFSM.Locomotive.rb.transform.forward);
            return;
        }
        if (trainFSM.CurrentRailLinker == null)
        {
            return;
        }

        

        TrainRailLinker.TrainData trainData;
        trainFSM.CurrentRailLinker.linkedRigidbodyObjects.TryGetValue(trainFSM.Locomotive.rb, out trainData);
        int index;
        if (trainData == null)
        {
            return;
        }
        else
        {
            index = trainData.followIndex;
            //index = Mathf.Min(trainData.followIndex, trainFSM.GetCurrentPath().path.localTangents.Length - 1);
        }

        trainFSM.Follow(trainFSM.GetCurrentPath().path.GetTangent(index));

        if ((trainFSM.GetNextRailLinker.WithinFollowDistance(0, trainFSM.Locomotive.rb.position)
            || trainFSM.CurrentRailLinker.WithinFollowDistance(trainFSM.CurrentRailLinker.pathCreator.path.localPoints.Length - 1, trainFSM.Locomotive.rb.position))
            && !trainFSM.CurrentRailLinker.pathCreator.path.isClosedLoop)
        {
            RailChangeMarker newMarker = new RailChangeMarker(trainFSM.carts, trainFSM.CurrentRailLinker, trainFSM.GetNextRailLinker);
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