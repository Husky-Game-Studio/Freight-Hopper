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

        return trainFSM.followPath;
    }

    public BasicState CheckFindNextPath()
    {
        // Waiting
        if (trainFSM.currentState == trainFSM.waiting && trainFSM.waiting.WaitingFinished())
        {
            return trainFSM.followPath;
        }
        return null;
    }
}