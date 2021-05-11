using System.Collections.Generic;
using System;

public class IdleState : PlayerState
{
    public IdleState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EnterState()
    {
        playerMachineCenter.abilities.jumpBehavior.coyoteeTimer.DeactivateTimer();
        playerMachineCenter.abilities.Recharge();
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();

        return state;
    }

    public override void PerformBehavior()
    {
    }
}