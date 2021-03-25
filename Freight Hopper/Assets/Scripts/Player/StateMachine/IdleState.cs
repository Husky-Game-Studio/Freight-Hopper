using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BasicState
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

    public BasicState DoState(PlayerMachineCenter playerMachine) {
        // Jump
        if (jumpPressed)
        {
            jumpPressed = false;
            //Debug.Log("Should be in Jump state!");
            return playerMachine.jumpState;
        }

        // if the <INSERT ABILITY> button was pressed, then go to <INSERT ABILITY> state (return myPlayer.<INSERT ABILITY>State)

        // if the run buttons was pressed, then go to run state (return myPlayer.runState)
        if (UserInput.Input.Move() != Vector2.zero)
        {
            //Debug.Log("Should be in Run state!");
            return playerMachine.runState;
        }
        else {
            //Debug.Log("In Idle state");
            return this;
        }
    }

    private void jumpButtonPressed()
    {
        jumpPressed = true;
    }
}
