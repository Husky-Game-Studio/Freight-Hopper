using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : BasicState
{
    private bool playerLanded = false;
    private bool jumpPressed = false;

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        // sub to Landed to trigger a function that returns a bool. and use that to pass or fail the if checks
        playerMachine.collision.Landed += this.HasLanded;
        UserInput.Input.JumpInput += this.JumpButtonPressed;

        if (playerMachine.GetPreviousState() != playerMachine.jumpState)
        {
            playerMachine.coyoteeTimer.ResetTimer();
        }
        else {
            playerMachine.coyoteeTimer.DeactivateTimer();
        }
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter) {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        playerMachine.collision.Landed -= this.HasLanded;
        UserInput.Input.JumpInput -= this.JumpButtonPressed;

        playerMachine.coyoteeTimer.DeactivateTimer();
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        // if coyetee timer is not expired and the jump button was pressed-> then jump

        // Jump
        //  THERE IS A BUG HERE ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        if (jumpPressed && playerMachine.coyoteeTimer.TimerActive() && playerMachine.GetPreviousState() != playerMachine.jumpState) {
            jumpPressed = false;
            return playerMachine.jumpState;
        }
        jumpPressed = false;
        // Fall
        if (!playerLanded)
        {
            //Debug.Log("In Fall state!: " + myPlayer.playerMovement.getIsGrounded().old);
            return this;
        }
        // Idle
        else
        {
            playerLanded = false;
            //Debug.Log("Should be in Idle state");
            return playerMachine.idleState;
        }
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        if (playerMachine.GetPreviousState() != playerMachine.jumpState) {
            playerMachine.coyoteeTimer.CountDown();
        }

        playerMachine.playerMovement.movement.Movement();
    }

    /*void PerformEntryBehavior(PlayerMachineCenter playerMachine) {
    
    }
    void PerformExitBehavior(PlayerMachineCenter playerMachine) {
    
    }*/

    private void HasLanded() {
        playerLanded = true;
    }

    public bool HasSubStateMachine() {
        return false;
    }

    private void JumpButtonPressed()
    {
        jumpPressed = true;
    }

    public BasicState GetCurrentSubState() { return null; }
    public BasicState[] GetSubStateArray() { return null; }
}
