using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FullStopState : BasicState
{
    private PlayerMachineCenter myPlayerMachineCenter;

    public FullStopState(List<Func<BasicState>> myTransitions) {
        this.stateTransitions = myTransitions;
    }

    public override void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter = playerMachine;

        playerMachine.abilities.fullstopBehavior.EntryAction();
    }

    public override void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter.abilities.fullstopBehavior.ExitAction();
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        foreach (Func<BasicState> stateCheck in this.stateTransitions) {
            BasicState tempState = stateCheck();
            if (tempState != null) {
                return tempState;
            }
        }


        // if (myPlayerMachineCenter.abilities.fullstopBehavior.FullStopFinished())
        // {
        //     return myPlayerMachineCenter.fallState;
        // }

        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter.abilities.fullstopBehavior.Action();
    }
}