using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpInitialState : BasicState
{
    //private bool releasedJumpPressed = false;

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        
        //UserInput.Input.JumpInputCanceled += this.ReleasedJumpButtonPressed;
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        
        //UserInput.Input.JumpInputCanceled -= this.ReleasedJumpButtonPressed;
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        return playerMachine.GetCurrentState().GetSubStateArray()[1];
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        playerMachine.playerMovement.movement.Movement();
        playerMachine.playerMovement.jumpBehavior.Jump(playerMachine.playerMovement.jumpBehavior.JumpHeight);
        //Debug.Log("In JumpInitialState at: " + Time.time);
    }

    /*private void ReleasedJumpButtonPressed()
    {
        releasedJumpPressed = true;
    }*/

    public bool HasSubStateMachine() { return false; }
    public BasicState GetCurrentSubState() { return null; }
    public BasicState[] GetSubStateArray() { return null; }
}
