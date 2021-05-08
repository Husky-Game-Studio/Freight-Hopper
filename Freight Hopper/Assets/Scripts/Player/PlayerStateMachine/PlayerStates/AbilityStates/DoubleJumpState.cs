using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DoubleJumpState : BasicState
{
    private PlayerMachineCenter myPlayerMachineCenter;

    public DoubleJumpState(List<Func<BasicState>> myTransitions) {
        this.stateTransitions = myTransitions;
    }

    public override void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter = playerMachine;
        //UserInput.Instance.JumpInputCanceled += this.ReleasedJumpButtonPressed;
        //UserInput.Instance.GrappleInput += this.GrappleButtonPressed;

        // reset jump hold timer
        playerMachine.jumpHoldingTimer.ResetTimer();
        myPlayerMachineCenter.abilities.doubleJumpBehavior.EntryAction();
    }

    public override void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        //UserInput.Instance.JumpInputCanceled -= this.ReleasedJumpButtonPressed;
        //UserInput.Instance.GrappleInput -= this.GrappleButtonPressed;

        // deactivate jump hold timer
        myPlayerMachineCenter.jumpHoldingTimer.DeactivateTimer();
        myPlayerMachineCenter.abilities.doubleJumpBehavior.ExitAction();
        playerMachine.pFSMTH.releasedJumpPressed = false;
        playerMachine.pFSMTH.grapplePressed = false;
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
        // if (releasedJumpPressed || !playerMachine.jumpHoldingTimer.TimerActive())
        // {
        //     playerMachine.jumpHoldingTimer.DeactivateTimer();
        //     return playerMachine.fallState;
        // }
        // Grapple pole
        // if (grapplePressed && !playerMachine.abilities.grapplePoleBehavior.Consumed && playerMachine.abilities.grapplePoleBehavior.Unlocked)
        // {
        //     return playerMachine.grapplePoleState;
        // }

        // Double Jump
        playerMachine.pFSMTH.releasedJumpPressed = false;
        playerMachine.pFSMTH.grapplePressed = false;
        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        //Debug.Log("Double Jump Performed on " + Time.time);
        // each fixedupdate the jump button is pressed down, this timer should decrease by that time
        myPlayerMachineCenter.jumpHoldingTimer.CountDownFixed();
        myPlayerMachineCenter.abilities.doubleJumpBehavior.Action();
    }
}