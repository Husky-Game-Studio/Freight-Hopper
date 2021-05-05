using UnityEngine;

public class WallJumpState : BasicState
{
    private PlayerMachineCenter myPlayerMachineCenter;

    public override void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter.abilities.wallRunBehavior.jumpHoldingTimer.ResetTimer();
        myPlayerMachineCenter.abilities.wallRunBehavior.WallJumpInitial();
    }

    public override void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter.abilities.wallRunBehavior.WallJumpContinous();
    }
}