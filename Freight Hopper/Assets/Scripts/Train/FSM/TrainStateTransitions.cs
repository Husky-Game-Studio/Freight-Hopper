using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainStateTransitions
{
    private TrainMachineCenter trainMachineCenter;

    public TrainStateTransitions(FiniteStateMachineCenter machineCenter)
    {
        trainMachineCenter = (TrainMachineCenter)machineCenter;
    }

    public BasicState CheckStartState()
    {
        if ((trainMachineCenter.StartWaitTime.Enabled && trainMachineCenter.StartWaitTime.value > 0) ||
            trainMachineCenter.StartWhenDistanceFromPlayer.Enabled && trainMachineCenter.StartWhenDistanceFromPlayer.value > 0)
        {
            return trainMachineCenter.waiting;
        }

        return trainMachineCenter.findNextPath;
    }
}