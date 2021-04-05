using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpHoldState : BasicState
{
    //private bool releasedJumpPressed = false;

    public void SubToListeners(PlayerMachineCenter playerMachine)
    {
        //UserInput.Input.JumpInputCanceled += this.ReleasedJumpButtonPressed;
    }

    public void UnsubToListeners(PlayerMachineCenter playerMachine)
    {
        //UserInput.Input.JumpInputCanceled -= this.ReleasedJumpButtonPressed;
    }

    public BasicState TransitionState(PlayerMachineCenter playerMachine)
    {
        return this;
    }

    public void PerformBehavior(PlayerMachineCenter playerMachine)
    {
        playerMachine.jumpHoldingTimer.CountDownFixed();
        playerMachine.playerMovement.movement.Movement();
        playerMachine.playerMovement.jumpBehavior.holdingJumpBehavior();
        Debug.Log("In JumpHoldState at: " + Time.time);
    }

    /*private void ReleasedJumpButtonPressed()
    {
        releasedJumpPressed = true;
    }*/

    public bool HasSubStateMachine() { return false; }
    public BasicState GetCurrentSubState() { return null; }
    public BasicState[] GetSubStateArray() { return null; }
}
