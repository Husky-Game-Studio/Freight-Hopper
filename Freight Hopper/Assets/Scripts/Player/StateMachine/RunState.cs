using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : BasicState
{
    private bool jumpPressed = false;

    public void SubToListeners(PlayerMachineCenter playerMachine)
    {
        UserInput.Input.JumpInput += this.JumpButtonPressed;
    }

    public void UnsubToListeners(PlayerMachineCenter playerMachine)
    {
        UserInput.Input.JumpInput -= this.JumpButtonPressed;
    }

    public BasicState TransitionState(PlayerMachineCenter playerMachine)
    {
        // if jumpbuffer timer is not expired, then jump

        
        // Jump
        if (jumpPressed)
        {
            jumpPressed = false;
            //Debug.Log("Should be in Jump state!");
            return playerMachine.jumpState;
        }

        // Run
        if (UserInput.Input.Move() != Vector2.zero)
        {
            //Debug.Log("in Run state!");
            return this;
        }
        // Idle
        else
        {
            //Debug.Log("Should be in Idle state");
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
