using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JumpState : BasicState
{
    private PlayerMachineCenter myPlayerMachineCenter;

    public JumpState(List<Func<BasicState>> myTransitions) {
        this.stateTransitions = myTransitions;
    }

    public override void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter = playerMachine;
        // UserInput.Instance.JumpInputCanceled += this.ReleasedJumpButtonPressed;
        // UserInput.Instance.GrappleInput += this.GrappleButtonPressed;

        // reset jump hold timer
        playerMachine.jumpHoldingTimer.ResetTimer();
        playerMachine.abilities.jumpBehavior.EntryAction();
    }

    public override void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        

        // UserInput.Instance.JumpInputCanceled -= this.ReleasedJumpButtonPressed;
        // UserInput.Instance.GrappleInput -= this.GrappleButtonPressed;

        // deactivate jump hold timer
        myPlayerMachineCenter.jumpHoldingTimer.DeactivateTimer();

        myPlayerMachineCenter.abilities.jumpBehavior.ExitAction();
        playerMachine.pFSMTH.releasedJumpPressed = false;
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
        
        
        // // Fall
        // if (releasedJumpPressed || !myPlayerMachineCenter.jumpHoldingTimer.TimerActive())
        // {
        //     return myPlayerMachineCenter.fallState;
        // }
        // Grapple pole
        // if (grapplePressed && !myPlayerMachineCenter.abilities.grapplePoleBehavior.Consumed && myPlayerMachineCenter.abilities.grapplePoleBehavior.Unlocked)
        // {
        //     return myPlayerMachineCenter.grapplePoleState;
        // }
        
        myPlayerMachineCenter.pFSMTH.jumpPressed = false;

        // Jump
        //else
        //{
            return this;
        //}
        
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        // each fixedupdate the jump button is pressed down, this timer should decrease by that time
        myPlayerMachineCenter.jumpHoldingTimer.CountDownFixed();

        myPlayerMachineCenter.abilities.jumpBehavior.Action();

        //playerMachine.jumpHoldingTimer.ResetTimer();
    }
}