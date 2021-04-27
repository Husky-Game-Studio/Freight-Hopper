using UnityEngine;

public class WallJumpState : BasicState
{
    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        Debug.Log("Entered Wall Jump State");
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        Debug.Log("exited Wall Jump State");
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        return this;
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        // Perform grapple pole behavior
        playerMachine.abilities.wallRunBehavior.WallJump();
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