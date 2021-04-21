using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPoundState : BasicState
{
    private bool jumpButtonPressed = false;
    private PlayerSubStateMachineCenter pSSMC;
    private PlayerMachineCenter myPlayerMachineCenter;
    private BasicState[] miniStateArray;

    public void SubToListeners(FiniteStateMachineCenter machineCenter) {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        UserInput.Input.JumpInput += this.JumpButtonPressed;

    }
    public void UnsubToListeners(FiniteStateMachineCenter machineCenter) {

        UserInput.Input.JumpInput -= this.JumpButtonPressed;
    }
    public BasicState TransitionState(FiniteStateMachineCenter machineCenter) {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        // if jump button is pressed and the jump ability has not already been used up // currently I do not have the one-use rule implemented
        if (jumpButtonPressed) {
            jumpButtonPressed = false;
            return playerMachine.jumpState;
        }

        return null;
    }
    public void PerformBehavior(FiniteStateMachineCenter machineCenter) {}
    public bool HasSubStateMachine() { return true; }
    public BasicState GetCurrentSubState() { return pSSMC.GetCurrentSubState(); }
    public BasicState[] GetSubStateArray() { return miniStateArray; }

    private void JumpButtonPressed(){
        jumpButtonPressed = true;
    }
}
