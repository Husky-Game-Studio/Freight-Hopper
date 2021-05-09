using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GroundPoundState : BasicState
{
    private PlayerMachineCenter myPlayerMachineCenter;

    public GroundPoundState(List<Func<BasicState>> myTransitions) {
        this.stateTransitions = myTransitions;
    }

    public override void EnterState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter = playerMachine;
        //UserInput.Instance.JumpInput += this.JumpButtonPressed;
        //UserInput.Instance.GroundPoundCanceled += this.GroundPoundButtonReleased;

        playerMachine.abilities.groundPoundBehavior.EntryAction();
    }

    public override void ExitState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        //UserInput.Instance.JumpInput -= this.JumpButtonPressed;
        //UserInput.Instance.GroundPoundCanceled -= this.GroundPoundButtonReleased;

        playerMachine.abilities.groundPoundBehavior.ExitAction();
        playerMachine.pFSMTH.groundPoundReleased = false;
        playerMachine.pFSMTH.jumpPressed = false;
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

        // Fall
        // if (groundPoundReleased)
        // {
        //     return playerMachine.fallState;
        // }

        // Jump
        // if (jumpPressed && !playerMachine.abilities.jumpBehavior.Consumed && playerMachine.abilities.jumpBehavior.Unlocked)
        // {
        //     return playerMachine.jumpState;
        // }
        // Double Jump
        // if (jumpPressed && playerMachine.abilities.jumpBehavior.Consumed && !playerMachine.abilities.doubleJumpBehavior.Consumed && playerMachine.abilities.doubleJumpBehavior.Unlocked)
        // {
        //     return playerMachine.doubleJumpState;
        // }
        // Grapple (SHOULDN'T CANCEL GROUND POUND)
        // Burst
        // Upward Dash
        playerMachine.pFSMTH.groundPoundReleased = false;
        playerMachine.pFSMTH.jumpPressed = false;
        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter.abilities.groundPoundBehavior.Action();
    }
}