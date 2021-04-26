using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpState : BasicState
{
    private bool grapplePressed = false;
    private bool releasedJumpPressed = false;
    private PlayerMachineCenter myPlayerMachineCenter;

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter = playerMachine;
        UserInput.Input.JumpInputCanceled += this.ReleasedJumpButtonPressed;
        UserInput.Input.GrappleInput += this.GrappleButtonPressed;

        // reset jump hold timer
        playerMachine.jumpHoldingTimer.ResetTimer();
        myPlayerMachineCenter.abilities.doubleJumpBehavior.EntryAction();
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        UserInput.Input.JumpInputCanceled -= this.ReleasedJumpButtonPressed;
        UserInput.Input.GrappleInput -= this.GrappleButtonPressed;

        // deactivate jump hold timer
        myPlayerMachineCenter.jumpHoldingTimer.DeactivateTimer();
        myPlayerMachineCenter.abilities.doubleJumpBehavior.ExitAction();
        releasedJumpPressed = false;
        grapplePressed = false;
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        // Fall
        if (releasedJumpPressed || !playerMachine.jumpHoldingTimer.TimerActive())
        {
            playerMachine.jumpHoldingTimer.DeactivateTimer();
            return playerMachine.fallState;
        }
        // Grapple pole
        if (grapplePressed)
        {
            return playerMachine.grapplePoleState;
        }

        // Double Jump
        releasedJumpPressed = false;
        grapplePressed = false;
        return this;
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        // each fixedupdate the jump button is pressed down, this timer should decrease by that time
        myPlayerMachineCenter.jumpHoldingTimer.CountDownFixed();
        myPlayerMachineCenter.abilities.doubleJumpBehavior.Action();
    }

    private void ReleasedJumpButtonPressed()
    {
        releasedJumpPressed = true;
    }

    private void GrappleButtonPressed()
    {
        grapplePressed = true;
    }

    public bool HasSubStateMachine()
    {
        return false;
    }

    public BasicState GetCurrentSubState()
    {
        return null;
    }

    public BasicState[] GetSubStateArray()
    {
        return null;
    }
}