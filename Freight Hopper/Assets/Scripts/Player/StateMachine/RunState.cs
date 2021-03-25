using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : BasicState
{
    private bool jumpPressed = false;

    public void subToListeners(PlayerMachineCenter playerMachine)
    {
        UserInput.Input.JumpInput += this.jumpButtonPressed;
    }

    public void unsubToListeners(PlayerMachineCenter playerMachine)
    {
        UserInput.Input.JumpInput -= this.jumpButtonPressed;
    }

    public BasicState DoState(PlayerMachineCenter playerMachine)
    {
        
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
    private void jumpButtonPressed()
    {
        jumpPressed = true;
    }
}
