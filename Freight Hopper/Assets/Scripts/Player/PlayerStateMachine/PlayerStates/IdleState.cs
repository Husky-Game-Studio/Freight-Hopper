using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BasicState
{
    private bool jumpPressed = false;
    private bool grapplePressed = false;

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        UserInput.Input.JumpInput += this.JumpButtonPressed;
        UserInput.Input.GrappleInput += this.GrappleButtonPressed;

        playerMachine.coyoteeTimer.DeactivateTimer();
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        UserInput.Input.JumpInput -= this.JumpButtonPressed;
        UserInput.Input.GrappleInput -= this.GrappleButtonPressed;
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        // Jump
        if (jumpPressed || playerMachine.jumpBufferTimer.TimerActive())
        {
            jumpPressed = false;
            return playerMachine.jumpState;
        }

        // Grapple pole
        if (grapplePressed)
        {
            grapplePressed = false;
            return playerMachine.grapplePoleState;
        }

        // Fall
        if (!playerMachine.collision.IsGrounded.current)
        {
            return playerMachine.fallState;
        }
        // Run
        if (UserInput.Input.Move() != Vector2.zero)
        {
            return playerMachine.runState;
        }
        // Idle
        else
        {
            return this;
        }
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        // reset coyotee timer, only decrements when in falling state // i dont think this comment is necessary anymore, i think it is solved somewhere else?
    }

    private void JumpButtonPressed()
    {
        jumpPressed = true;
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