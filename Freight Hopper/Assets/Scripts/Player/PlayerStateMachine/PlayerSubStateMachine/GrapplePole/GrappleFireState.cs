using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleFireState : BasicState
{
    private bool startOfGrapple = true;

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        startOfGrapple = true;
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        if (playerMachine.playerMovement.grapplePoleBehavior.Anchored())
        {
            return playerMachine.GetCurrentState().GetSubStateArray()[1];
        }

        return this;
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        // Call animation and mesh generation
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        playerMachine.playerMovement.movementBehavior.Movement();
        if (startOfGrapple)
        {
            playerMachine.playerMovement.grapplePoleBehavior.StartGrapple();
            startOfGrapple = false;
        }
        playerMachine.playerMovement.grapplePoleBehavior.GrappleTransition();
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