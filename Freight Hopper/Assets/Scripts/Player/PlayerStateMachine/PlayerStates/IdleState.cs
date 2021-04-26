using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BasicState
{
    private bool jumpPressed = false;
    private bool grapplePressed = false;
    private bool groundPoundPressed = false;

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        UserInput.Input.JumpInput += this.JumpButtonPressed;
        UserInput.Input.GrappleInput += this.GrappleButtonPressed;
        UserInput.Input.GroundPoundInput += this.GroundPoundButtonPressed;

        playerMachine.coyoteeTimer.DeactivateTimer();
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        UserInput.Input.JumpInput -= this.JumpButtonPressed;
        UserInput.Input.GrappleInput -= this.GrappleButtonPressed;
        UserInput.Input.GroundPoundInput -= this.GroundPoundButtonPressed;
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

        // Double Jump
        if (jumpPressed && playerMachine.playerAbilities.jumpBehavior.IsConsumed && !playerMachine.playerAbilities.doubleJumpBehavior.IsConsumed)
        {
            jumpPressed = false;
            return playerMachine.doubleJumpState;
        }

        // Grapple pole
        if (grapplePressed)
        {
            grapplePressed = false;
            return playerMachine.grapplePoleState;
        }

        // Ground Pound
        if (groundPoundPressed &&
            (playerMachine.playerCM.ContactNormal.current != playerMachine.playerCM.ValidUpAxis ||
            playerMachine.playerCM.IsGrounded.current == false) && !playerMachine.playerAbilities.groundPoundBehavior.IsConsumed)
        {
            groundPoundPressed = false;
            return playerMachine.groundPoundState;
        }
        groundPoundPressed = false;
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

    private void GroundPoundButtonPressed()
    {
        groundPoundPressed = true;
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