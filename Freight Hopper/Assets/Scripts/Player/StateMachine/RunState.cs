using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : BasicState
{
    private bool jumpPressed = false;

    public void SubToListeners(PlayerMachineCenter playerMachine)
    {
        UserInput.Input.JumpInput += this.JumpButtonPressed;

        playerMachine.coyoteeTimer.DeactivateTimer();
    }

    public void UnsubToListeners(PlayerMachineCenter playerMachine)
    {
        UserInput.Input.JumpInput -= this.JumpButtonPressed;
    }

    public BasicState TransitionState(PlayerMachineCenter playerMachine)
    {
        // Jump
        if (jumpPressed || playerMachine.jumpBufferTimer.TimerActive())
        {
            jumpPressed = false;
            return playerMachine.jumpState;
        }
        // Fall
        if (!playerMachine.collision.IsGrounded.current)
        {
            return playerMachine.fallState;
        }
        // Run
        if (UserInput.Input.Move() != Vector2.zero)
        {
            return this;
        }
        // Idle
        else
        {
            return playerMachine.idleState;
        }
    }

    public void PerformBehavior(PlayerMachineCenter playerMachine)
    {
        playerMachine.playerMovement.movement.Movement();
    }

    private void JumpButtonPressed()
    {
        jumpPressed = true;
    }

    public bool HasSubStateMachine()
    {
        return false;
    }
    public BasicState GetCurrentSubState() { return null; }
    public BasicState[] GetSubStateArray() { return null; }
}
