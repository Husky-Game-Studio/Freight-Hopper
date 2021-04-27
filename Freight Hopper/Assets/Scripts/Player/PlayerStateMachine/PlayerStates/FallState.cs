using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : BasicState
{
    private bool jumpPressed = false;
    private bool grapplePressed = false;
    private bool groundPoundPressed = false;
    private bool upwardDashPressed = false;
    private bool fullStopPressed = false;
    private bool burstPressed = false;

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        UserInput.Input.JumpInput += this.JumpButtonPressed;
        UserInput.Input.GrappleInput += this.GrappleButtonPressed;
        UserInput.Input.GroundPoundInput += this.GroundPoundButtonPressed;
        UserInput.Input.UpwardDashInput += this.UpwardDashPressed;
        UserInput.Input.FullStopInput += this.FullStopPressed;
        UserInput.Input.BurstInput += this.BurstPressed;

        if (playerMachine.GetPreviousState() != playerMachine.jumpState)
        {
            playerMachine.coyoteeTimer.ResetTimer();
        }
        else
        {
            playerMachine.coyoteeTimer.DeactivateTimer();
        }
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        UserInput.Input.JumpInput -= this.JumpButtonPressed;
        UserInput.Input.GrappleInput -= this.GrappleButtonPressed;
        UserInput.Input.GroundPoundInput -= this.GroundPoundButtonPressed;
        UserInput.Input.UpwardDashInput -= this.UpwardDashPressed;
        UserInput.Input.FullStopInput -= this.FullStopPressed;
        UserInput.Input.BurstInput -= this.BurstPressed;

        playerMachine.coyoteeTimer.DeactivateTimer();
        jumpPressed = false;
        groundPoundPressed = false;
        grapplePressed = false;
        upwardDashPressed = false;
        fullStopPressed = false;
        burstPressed = false;
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        // if coyetee timer is not expired and the jump button was pressed-> then jump

        // Jump
        //  THERE IS A BUG HERE ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        if (jumpPressed && playerMachine.coyoteeTimer.TimerActive() && playerMachine.GetPreviousState() != playerMachine.jumpState &&
            !playerMachine.abilities.jumpBehavior.IsConsumed)
        {
            return playerMachine.jumpState;
        }
        // Double Jump
        if (jumpPressed && !playerMachine.abilities.doubleJumpBehavior.IsConsumed)
        {
            return playerMachine.doubleJumpState;
        }

        // Ground Pound
        if (groundPoundPressed &&
            (playerMachine.playerCM.ContactNormal.current != playerMachine.playerCM.ValidUpAxis ||
            playerMachine.playerCM.IsGrounded.current == false) && !playerMachine.abilities.groundPoundBehavior.IsConsumed)
        {
            return playerMachine.groundPoundState;
        }
        // Grapple pole
        if (grapplePressed && !playerMachine.abilities.grapplePoleBehavior.IsConsumed)
        {
            return playerMachine.grapplePoleState;
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

        // Burst
        if (burstPressed && !playerMachine.abilities.burstBehavior.IsConsumed)
        {
            return playerMachine.burstState;
        }

        // Idle
        if (playerMachine.playerCM.IsGrounded.current)
        {
            foreach (AbilityBehavior ability in playerMachine.abilities.Abilities)
            {
                ability.Recharge();
            }
            return playerMachine.idleState;
        }

        // Wall run
        if (playerMachine.abilities.wallRunBehavior.IsUnlocked)
        {
            bool[] walls = playerMachine.abilities.wallRunBehavior.CheckWalls();
            if (walls[0] || walls[1] || walls[3])
            {
                return playerMachine.wallRunState;
            }
        }

        jumpPressed = false;
        groundPoundPressed = false;
        grapplePressed = false;
        upwardDashPressed = false;
        fullStopPressed = false;
        burstPressed = false;
        return this;
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        if (playerMachine.GetPreviousState() != playerMachine.jumpState)
        {
            playerMachine.coyoteeTimer.CountDown();
        }

        playerMachine.abilities.movementBehavior.Action();
    }

    public bool HasSubStateMachine()
    {
        return false;
    }

    private void JumpButtonPressed()
    {
        jumpPressed = true;
    }

    private void GroundPoundButtonPressed()
    {
        groundPoundPressed = true;
    }

    private void UpwardDashPressed()
    {
        upwardDashPressed = true;
    }

    private void GrappleButtonPressed()
    {
        grapplePressed = true;
    }

    private void FullStopPressed()
    {
        fullStopPressed = true;
    }

    private void BurstPressed()
    {
        burstPressed = true;
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