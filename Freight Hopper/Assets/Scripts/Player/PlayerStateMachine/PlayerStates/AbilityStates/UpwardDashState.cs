using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpwardDashState : BasicState
{
    private bool grapplePressed = false;
    private bool releasedUpwardDash = false;
    private PlayerMachineCenter myPlayerMachineCenter;

    public override void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter = playerMachine;
        UserInput.Instance.UpwardDashInputCanceled += this.ReleasedUpwardDash;
        UserInput.Instance.GrappleInput += this.GrappleButtonPressed;

        playerMachine.abilities.upwardDashBehavior.EntryAction();
    }

    public override void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        UserInput.Instance.UpwardDashInputCanceled -= this.ReleasedUpwardDash;
        UserInput.Instance.GrappleInput -= this.GrappleButtonPressed;

        myPlayerMachineCenter.abilities.upwardDashBehavior.ExitAction();
        releasedUpwardDash = false;
        grapplePressed = false;
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        // Fall
        if (releasedUpwardDash)
        {
            return myPlayerMachineCenter.fallState;
        }
        // Grapple pole
        if (grapplePressed && !myPlayerMachineCenter.abilities.grapplePoleBehavior.Consumed)
        {
            return myPlayerMachineCenter.grapplePoleState;
        }

        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
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
}