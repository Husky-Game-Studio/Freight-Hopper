using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainStateTransitions
{
    private TrainMachineCenter trainFSM;

    public TrainStateTransitions(FiniteStateMachineCenter machineCenter)
    {
        trainFSM = (TrainMachineCenter)machineCenter;
    }

    public BasicState CheckStartState()
    {
        if (trainFSM.StartWaitTime.Enabled ||
            trainFSM.StartWhenDistanceFromPlayer.Enabled)
        {
            return trainFSM.waiting;
        }

        return trainFSM.findNextPath;
    }

    public BasicState CheckFindNextPath()
    {
        // Waiting
        if (trainFSM.currentState == trainFSM.waiting && trainFSM.waiting.WaitingFinished())
        {
            return trainFSM.findNextPath;
        }
        // Follow Path
        if (trainFSM.currentState == trainFSM.followPath && (trainFSM.followPath.EndOfPath && !trainFSM.OnFinalPath))
        {
            return trainFSM.findNextPath;
        }
        return null;
    }

    public BasicState CheckFollowPath()
    {
        // Find Next Path
        if (trainFSM.currentState == trainFSM.findNextPath && trainFSM.findNextPath.ReachedTarget())
        {
            return trainFSM.followPath;
        }
        return null;
    }

    public BasicState CheckWander()
    {
        // Follow Path
        if (trainFSM.currentState == trainFSM.followPath &&
            (trainFSM.followPath.EndOfPath && trainFSM.OnFinalPath) ||
            ((trainFSM.rb[0].position - trainFSM.followPath.TargetPos).magnitude > trainFSM.DerailDistance))
        {
            return trainFSM.wander;
        }
        return null;
    }
}