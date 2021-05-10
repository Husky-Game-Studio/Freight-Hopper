using System;
using System.Collections.Generic;

public class WallJumpState : PlayerState
{
    public WallJumpState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EnterState()
    {
        playerMachineCenter.abilities.wallRunBehavior.jumpHoldingTimer.ResetTimer();
        playerMachineCenter.abilities.wallRunBehavior.WallJumpInitial();
    }

    public override BasicState TransitionState()
    {
        return this;
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.abilities.wallRunBehavior.WallJumpContinous();
    }
}