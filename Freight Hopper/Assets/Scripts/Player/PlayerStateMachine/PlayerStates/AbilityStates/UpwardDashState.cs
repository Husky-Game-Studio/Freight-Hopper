using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UpwardDashState : BasicState
{
    private PlayerMachineCenter myPlayerMachineCenter;

    public UpwardDashState(List<Func<BasicState>> myTransitions) {
        this.stateTransitions = myTransitions;
    }

    public override void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter = playerMachine;
        // UserInput.Instance.UpwardDashInputCanceled += this.ReleasedUpwardDash;
        // UserInput.Instance.GrappleInput += this.GrappleButtonPressed;

        playerMachine.abilities.upwardDashBehavior.EntryAction();
    }

    public override void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        // UserInput.Instance.UpwardDashInputCanceled -= this.ReleasedUpwardDash;
        // UserInput.Instance.GrappleInput -= this.GrappleButtonPressed;

        myPlayerMachineCenter.abilities.upwardDashBehavior.ExitAction();
        playerMachine.pFSMTH.releasedUpwardDash = false;
        playerMachine.pFSMTH.grapplePressed = false;
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {

        foreach (Func<BasicState> stateCheck in this.stateTransitions) {
            BasicState tempState = stateCheck();
            if (tempState != null) {
                return tempState;
            }
        }

        // Fall
        // if (releasedUpwardDash)
        // {
        //     return myPlayerMachineCenter.fallState;
        // }
        // Grapple pole
        // if (grapplePressed && !myPlayerMachineCenter.abilities.grapplePoleBehavior.Consumed)
        // {
        //     return myPlayerMachineCenter.grapplePoleState;
        // }

        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter.abilities.upwardDashBehavior.Action();
    }
}