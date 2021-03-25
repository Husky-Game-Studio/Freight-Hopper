using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : BasicState
{
    private bool releasedJumpPressed = false;

    public void subToListeners(PlayerMachineCenter playerMachine)
    {
        UserInput.Input.JumpInputCanceled += this.releasedJumpButtonPressed;
    }

    public void unsubToListeners(PlayerMachineCenter playerMachine)
    {
        UserInput.Input.JumpInputCanceled -= this.releasedJumpButtonPressed;
    }

    public BasicState DoState(PlayerMachineCenter playerMachine)
    {
        
        // Fall
        if (releasedJumpPressed || !playerMachine.playerMovement.jumpBehavior.IsJumping)
        {
            
            //Debug.Log("Should be in Fall state");
            releasedJumpPressed = false;
            
            return playerMachine.fallState;
        }
        // Jump
        else
        {
            //Debug.Log("In Jump state!");
            
            return this;
        }
    }

    private void releasedJumpButtonPressed()
    {
        releasedJumpPressed = true;
    }
}
