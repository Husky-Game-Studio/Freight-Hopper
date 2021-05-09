using UnityEngine;

public class WallClimbingState : BasicState
{
    public override void EnterState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        playerMachine.abilities.wallRunBehavior.InitialWallClimb();
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