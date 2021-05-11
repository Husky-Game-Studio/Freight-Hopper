using System.Collections.Generic;
using System;

public class RunState : PlayerState
{
    public RunState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EnterState()
    {
        playerMachineCenter.abilities.jumpBehavior.coyoteeTimer.DeactivateTimer();
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();

        return state;
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.abilities.movementBehavior.Action();
    }
}