using System.Collections.Generic;
using System;
using UnityEngine;

public class WaitingState : BasicState
{
    private Timer waitTime;
    private TrainMachineCenter trainFSM;
    private Transform playerTransform;
    private Transform locomotive;

    public WaitingState(FiniteStateMachineCenter machineCenter, List<Func<BasicState>> stateTransitions) : base(machineCenter, stateTransitions)
    {
        this.trainFSM = (TrainMachineCenter)machineCenter;
    }

    public bool WaitingFinished()
    {
        if (trainFSM.StartWaitTime.Enabled && !trainFSM.StartWhenDistanceFromPlayer.Enabled)
        {
            return !waitTime.TimerActive();
        }
        if (!trainFSM.StartWaitTime.Enabled && trainFSM.StartWhenDistanceFromPlayer.Enabled)
        {
            return Vector3.Distance(playerTransform.position, locomotive.position) <= trainFSM.StartWhenDistanceFromPlayer.value;
        }
        if (trainFSM.StartWaitTime.Enabled && trainFSM.StartWhenDistanceFromPlayer.Enabled)
        {
            return !waitTime.TimerActive() ||
                Vector3.Distance(playerTransform.position, locomotive.position) <= trainFSM.StartWhenDistanceFromPlayer.value;
        }
        if (trainFSM.StartOnTriggerEnter.Enabled && trainFSM.StartOnTriggerEnter.value != null)
        {
            return trainFSM.IsTriggerEntered;
        }
        return true;
    }

    public override void EntryState()
    {
        if (trainFSM.StartWaitTime.Enabled)
        {
            waitTime = new Timer(trainFSM.StartWaitTime.value);
            waitTime.ResetTimer();
        }
        if (trainFSM.StartWhenDistanceFromPlayer.Enabled)
        {
            playerTransform = Player.Instance.transform;
            locomotive = trainFSM.transform;
        }
        if (trainFSM.StartOnTriggerEnter.Enabled && trainFSM.StartOnTriggerEnter.value != null)
        {
            trainFSM.StartOnTriggerEnter.value.triggered += trainFSM.EnteredTrigger;
        }
    }

    public override void ExitState()
    {
        if (trainFSM.StartOnTriggerEnter.Enabled && trainFSM.StartOnTriggerEnter.value != null)
        {
            trainFSM.StartOnTriggerEnter.value.triggered -= trainFSM.EnteredTrigger;
        }
    }

    public override void PerformBehavior()
    {
        if (trainFSM.StartWaitTime.Enabled)
        {
            waitTime.CountDownFixed();
        }
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();
        return state;
    }
}