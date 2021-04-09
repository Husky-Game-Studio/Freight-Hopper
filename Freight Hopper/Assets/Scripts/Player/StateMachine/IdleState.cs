using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BasicState
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

    public BasicState TransitionState(PlayerMachineCenter playerMachine) {
        // Jump
        if (jumpPressed || playerMachine.jumpBufferTimer.TimerActive())
        {
            jumpPressed = false;
            return playerMachine.jumpState;
        }
        // Fall
        if (!playerMachine.collision.IsGrounded.current) {
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

    public void PerformBehavior(PlayerMachineCenter playerMachine)
    {
        // reset coyotee timer, only decrements when in falling state
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
