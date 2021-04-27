using UnityEngine;

public class WallClimbingState : BasicState
{
    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        Debug.Log("Entered Climbing State");
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        return this;
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        playerMachine.abilities.wallRunBehavior.WallClimb();
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