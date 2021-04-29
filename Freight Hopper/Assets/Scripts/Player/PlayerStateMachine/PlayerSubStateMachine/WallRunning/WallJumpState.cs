using UnityEngine;

public class WallJumpState : BasicState
{
    public override void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        Debug.Log("Entered Wall Jump State");
    }

    public override void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        Debug.Log("exited Wall Jump State");
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        // Perform grapple pole behavior
        playerMachine.abilities.wallRunBehavior.WallJump();
    }
}