using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : BasicState
{
    private bool releasedJumpPressed = false;
    private SubStateMachineCenter mySSMC;
    private PlayerMachineCenter myPlayerMachineCenter;
    private BasicState[] miniStateArray;

    public JumpState(PlayerMachineCenter myPMC) {
        myPlayerMachineCenter = myPMC;
        miniStateArray = new BasicState[2];
        miniStateArray[0] = new JumpInitialState();
        miniStateArray[1] = new JumpHoldState();
        mySSMC = new SubStateMachineCenter(this, miniStateArray, myPlayerMachineCenter);
    }

    public void SubToListeners(PlayerMachineCenter playerMachine)
    {
        UserInput.Input.JumpInputCanceled += this.ReleasedJumpButtonPressed;
        //UserInput.Input.JumpInput += playerMachine.playerMovement.jumpBehavior.TryJump;
        mySSMC.GetCurrentSubState().SubToListeners(playerMachine);
    }

    public void UnsubToListeners(PlayerMachineCenter playerMachine)
    {
        UserInput.Input.JumpInputCanceled -= this.ReleasedJumpButtonPressed;
        //UserInput.Input.JumpInput -= playerMachine.playerMovement.jumpBehavior.TryJump;
        mySSMC.GetCurrentSubState().UnsubToListeners(playerMachine);

    }

    public BasicState TransitionState(PlayerMachineCenter playerMachine)
    {
        // Fall
        if (releasedJumpPressed || !playerMachine.playerMovement.jumpBehavior.IsJumping)
        {
            releasedJumpPressed = false;
            return playerMachine.fallState;
        }
        // Jump
        else
        {
            return this;
        }
    }

    public void PerformBehavior(PlayerMachineCenter playerMachine)
    {
        // deactivate jump buffer and coyotee timer


        // Perform the SubStateMachine Behavior
        mySSMC.PerformSubMachineBehavior();

        // THIS SHOULD BE DONE IN THE SUBSTATEMACHINE
        //playerMachine.playerMovement.movement.Movement();
        //playerMachine.playerMovement.jumpBehavior.Jumping();
    }

    private void ReleasedJumpButtonPressed()
    {
        releasedJumpPressed = true;
    }

    public bool HasSubStateMachine()
    {
        return true;
    }

    public BasicState GetCurrentSubState() { return mySSMC.GetCurrentSubState(); }

    public BasicState[] GetSubStateArray() { return miniStateArray; }
}
