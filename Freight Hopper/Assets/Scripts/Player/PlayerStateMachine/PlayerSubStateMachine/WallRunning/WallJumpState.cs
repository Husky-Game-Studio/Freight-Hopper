using System;
using System.Collections.Generic;

public class WallJumpState : PlayerState
{
    public WallJumpState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EntryState()
    {
        playerMachineCenter.wallRunBehavior.jumpHoldingTimer.ResetTimer();
        playerMachineCenter.wallRunBehavior.WallJumpInitial();
    }

    public override BasicState TransitionState()
    {
        return this;
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.wallRunBehavior.WallJumpContinous();
    }
}