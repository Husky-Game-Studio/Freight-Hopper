using System;
using System.Collections.Generic;

public class GrapplePoleAnchoredState : PlayerState
{
    public GrapplePoleAnchoredState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override BasicState TransitionState()
    {
        return CheckTransitions();
    }

    public override void PerformBehavior()
    {
    }
}