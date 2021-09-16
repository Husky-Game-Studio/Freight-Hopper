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
        if (trainFSM.OnFinalPath ||
            trainFSM.StartWaitTime.Enabled ||
            trainFSM.StartWhenDistanceFromPlayer.Enabled ||
            (trainFSM.StartOnTriggerEnter.Enabled && trainFSM.StartOnTriggerEnter.value != null))
        {
            return trainFSM.waiting;
        }
        return trainFSM.followPath;
    }

    public BasicState CheckWaiting()
    {
        if (!trainFSM.waiting.WaitingFinished() && !trainFSM.waiting.WaitedAtStart)
        {
            return trainFSM.waiting;
        }
        if (trainFSM.CompletedPaths)
        {
            return trainFSM.waiting;
        }
        return null;
    }

    public BasicState CheckFollowPath()
    {
        if (trainFSM.waiting.WaitingFinished() && !trainFSM.waiting.WaitedAtStart)
        {
            return trainFSM.followPath;
        }
        return null;
    }
}