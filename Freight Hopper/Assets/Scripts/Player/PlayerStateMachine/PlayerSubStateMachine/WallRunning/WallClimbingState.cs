using UnityEngine;

public class WallClimbingState : BasicState
{
    public override void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        Debug.Log("Entered Climbing State");
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        playerMachine.abilities.wallRunBehavior.WallClimb();
    }
}