using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : BasicState
{
    private bool jumpPressed = false;
    private bool grapplePressed = false;
    private bool groundPoundPressed = false;

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        // sub to Landed to trigger a function that returns a bool. and use that to pass or fail the if checks
        UserInput.Input.JumpInput += this.JumpButtonPressed;
        UserInput.Input.GrappleInput += this.GrappleButtonPressed;
        UserInput.Input.GroundPoundInput += this.GroundPoundButtonPressed;

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

        playerMachine.coyoteeTimer.DeactivateTimer();
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        // if coyetee timer is not expired and the jump button was pressed-> then jump

        // Jump
        //  THERE IS A BUG HERE ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        if (jumpPressed && playerMachine.coyoteeTimer.TimerActive() && playerMachine.GetPreviousState() != playerMachine.jumpState)
        {
            jumpPressed = false;
            return playerMachine.jumpState;
        }
        // Double Jump
        if (jumpPressed && playerMachine.playerAbilities.jumpBehavior.IsConsumed && !playerMachine.playerAbilities.doubleJumpBehavior.IsConsumed)
        {
            jumpPressed = false;
            return playerMachine.doubleJumpState;
        }
        jumpPressed = false;

        // Ground Pound
        if (groundPoundPressed &&
            (playerMachine.playerCM.ContactNormal.current != playerMachine.playerCM.ValidUpAxis ||
            playerMachine.playerCM.IsGrounded.current == false) && !playerMachine.playerAbilities.groundPoundBehavior.IsConsumed)
        {
            groundPoundPressed = false;
            return playerMachine.groundPoundState;
        }
        groundPoundPressed = false;
        // Grapple pole
        if (grapplePressed)
        {
            grapplePressed = false;
            return playerMachine.grapplePoleState;
        }

        // Fall
        if (!playerMachine.playerCM.IsGrounded.current)
        {
            return this;
        }
        // Idle
        else
        {
            foreach (AbilityBehavior ability in playerMachine.playerAbilities.Abilities)
            {
                ability.Recharge();
            }
            return playerMachine.idleState;
        }
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        if (playerMachine.GetPreviousState() != playerMachine.jumpState)
        {
            playerMachine.coyoteeTimer.CountDown();
        }

        playerMachine.playerAbilities.movementBehavior.Action();
    }

    /*void PerformEntryBehavior(PlayerMachineCenter playerMachine) {
    }
    void PerformExitBehavior(PlayerMachineCenter playerMachine) {
    }*/

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

    private void GrappleButtonPressed()
    {
        grapplePressed = true;
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