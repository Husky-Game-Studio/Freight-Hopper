using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpwardDashState : BasicState
{
    private bool grapplePressed = false;
    private bool releasedUpwardDash = false;
    private PlayerMachineCenter myPlayerMachineCenter;

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter = playerMachine;
        UserInput.Input.UpwardDashInputCanceled += this.ReleasedUpwardDash;
        UserInput.Input.GrappleInput += this.GrappleButtonPressed;

        playerMachine.abilities.upwardDashBehavior.EntryAction();
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        UserInput.Input.UpwardDashInputCanceled -= this.ReleasedUpwardDash;
        UserInput.Input.GrappleInput -= this.GrappleButtonPressed;

        myPlayerMachineCenter.abilities.upwardDashBehavior.ExitAction();
        releasedUpwardDash = false;
        grapplePressed = false;
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        // Fall
        if (releasedUpwardDash)
        {
            return myPlayerMachineCenter.fallState;
        }
        // Grapple pole
        if (grapplePressed && !myPlayerMachineCenter.abilities.grapplePoleBehavior.IsConsumed)
        {
            return myPlayerMachineCenter.grapplePoleState;
        }

        return this;
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter.abilities.upwardDashBehavior.Action();
    }

    private void ReleasedUpwardDash()
    {
        releasedUpwardDash = true;
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