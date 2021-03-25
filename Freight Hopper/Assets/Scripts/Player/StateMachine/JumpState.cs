using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : BasicState
{
    private bool releasedJumpPressed = false;

    public void SubToListeners(PlayerMachineCenter playerMachine)
    {
        UserInput.Input.JumpInputCanceled += this.ReleasedJumpButtonPressed;
    }

    public void UnsubToListeners(PlayerMachineCenter playerMachine)
    {
        UserInput.Input.JumpInputCanceled -= this.ReleasedJumpButtonPressed;
    }

    public BasicState TransitionState(PlayerMachineCenter playerMachine)
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

    public void PerformBehavior(PlayerMachineCenter playerMachine)
    {

    }

    private void ReleasedJumpButtonPressed()
    {
        releasedJumpPressed = true;
    }
}
