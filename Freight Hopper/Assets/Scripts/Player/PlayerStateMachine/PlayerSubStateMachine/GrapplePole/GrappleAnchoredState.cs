using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleAnchoredState : BasicState
{
    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        return this;
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        // Perform grapple pole behavior
        playerMachine.playerMovement.grapplePoleBehavior.Grapple();
        // REMOVE THE MOVEMENT LATER, SHOULDN'T BE ABLE TO MOVE
        playerMachine.playerMovement.movementBehavior.Movement();
    }

    public bool HasSubStateMachine()
    {
        return false;
    }

    public BasicState GetCurrentSubState()
    {
        return null;
    }

    public BasicState[] GetSubStateArray()
    {
        return null;
    }
}