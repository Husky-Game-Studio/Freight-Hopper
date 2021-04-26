using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BasicState
{
    private bool jumpPressed = false;
    private bool grapplePressed = false;
    private bool groundPoundPressed = false;
    private bool upwardDashPressed = false;
    private bool fullStopPressed = false;

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        UserInput.Input.JumpInput += this.JumpPressed;
        UserInput.Input.GrappleInput += this.GrapplePressed;
        UserInput.Input.GroundPoundInput += this.GroundPoundPressed;
        UserInput.Input.UpwardDashInput += this.UpwardDashPressed;
        UserInput.Input.FullStopInput += this.FullStopPressed;

        playerMachine.coyoteeTimer.DeactivateTimer();
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        UserInput.Input.JumpInput -= this.JumpPressed;
        UserInput.Input.GrappleInput -= this.GrapplePressed;
        UserInput.Input.GroundPoundInput -= this.GroundPoundPressed;
        UserInput.Input.UpwardDashInput -= this.UpwardDashPressed;
        UserInput.Input.FullStopInput -= this.FullStopPressed;
        jumpPressed = false;
        grapplePressed = false;
        groundPoundPressed = false;
        upwardDashPressed = false;
        fullStopPressed = false;
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        // Jump
        if ((jumpPressed || playerMachine.jumpBufferTimer.TimerActive()) && !playerMachine.abilities.jumpBehavior.IsConsumed)
        {
            return playerMachine.jumpState;
        }

        // Double Jump
        if (jumpPressed && playerMachine.abilities.jumpBehavior.IsConsumed && !playerMachine.abilities.doubleJumpBehavior.IsConsumed)
        {
            return playerMachine.doubleJumpState;
        }

        // Grapple pole
        if (grapplePressed && !playerMachine.abilities.grapplePoleBehavior.IsConsumed)
        {
            playerMachine.abilities.grapplePoleBehavior.PreventConsumption();
            return playerMachine.grapplePoleState;
        }

        // Ground Pound
        if (groundPoundPressed &&
            (playerMachine.playerCM.ContactNormal.current != playerMachine.playerCM.ValidUpAxis ||
            playerMachine.playerCM.IsGrounded.current == false) && !playerMachine.abilities.groundPoundBehavior.IsConsumed)
        {
            return playerMachine.groundPoundState;
        }

        // Upward Dash
        if (upwardDashPressed && !playerMachine.abilities.upwardDashBehavior.IsConsumed)
        {
            return playerMachine.upwardDashState;
        }

        // Full Stop
        if (fullStopPressed && !playerMachine.abilities.fullstopBehavior.IsConsumed)
        {
            return playerMachine.fullStopState;
        }

        // Fall
        if (!playerMachine.playerCM.IsGrounded.current)
        {
            return playerMachine.fallState;
        }
        // Run
        if (UserInput.Input.Move() != Vector3.zero)
        {
            return playerMachine.runState;
        }

        jumpPressed = false;
        grapplePressed = false;
        groundPoundPressed = false;
        upwardDashPressed = false;
        fullStopPressed = false;
        return this;
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        // reset coyotee timer, only decrements when in falling state // i dont think this comment is necessary anymore, i think it is solved somewhere else?
    }

    private void JumpPressed()
    {
        jumpPressed = true;
    }

    private void GrapplePressed()
    {
        grapplePressed = true;
    }

    private void GroundPoundPressed()
    {
        groundPoundPressed = true;
    }

    private void UpwardDashPressed()
    {
        upwardDashPressed = true;
    }

    private void FullStopPressed()
    {
        fullStopPressed = true;
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