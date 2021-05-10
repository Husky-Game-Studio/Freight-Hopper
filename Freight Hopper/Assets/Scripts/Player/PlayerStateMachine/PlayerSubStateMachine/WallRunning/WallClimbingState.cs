using System;
using System.Collections.Generic;

public class WallClimbingState : PlayerState
{
    public WallClimbingState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EnterState()
    {
        playerMachineCenter.abilities.wallRunBehavior.InitialWallClimb();
    }

    public override BasicState TransitionState()
    {
        return this;
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.abilities.wallRunBehavior.WallClimb();
    }
}