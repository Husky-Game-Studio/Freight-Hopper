using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GrappleFireState : BasicState
{
    private bool startOfGrapple = true;

    public GrappleFireState(List<Func<BasicState>> myTransitions) {
        this.stateTransitions = myTransitions;
    }

    public override void EnterState(FiniteStateMachineCenter machineCenter)
    {
        startOfGrapple = true;
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        
        foreach (Func<BasicState> stateCheck in this.stateTransitions) {
            BasicState tempState = stateCheck();
            if (tempState != null) {
                return tempState;
            }
        }

        // if (playerMachine.abilities.grapplePoleBehavior.IsAnchored())
        // {
        //     return playerMachine.GetCurrentState().GetSubStateArray()[1];
        // }

        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        // Call animation and mesh generation
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        playerMachine.abilities.movementBehavior.PlayerMove();
        if (startOfGrapple)
        {
            playerMachine.abilities.grapplePoleBehavior.EntryAction();
            startOfGrapple = false;
        }
        playerMachine.abilities.grapplePoleBehavior.GrappleTransition();
    }
}