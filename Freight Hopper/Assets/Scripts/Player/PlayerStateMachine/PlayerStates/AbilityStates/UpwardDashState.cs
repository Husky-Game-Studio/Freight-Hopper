using System.Collections.Generic;
using System;

public class UpwardDashState : PlayerState
{
    public UpwardDashState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EntryState()
    {
        playerMachineCenter.abilities.upwardDashBehavior.EntryAction();
    }

    public override void ExitState()
    {
        playerMachineCenter.abilities.upwardDashBehavior.ExitAction();
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();

        return state;
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.abilities.upwardDashBehavior.Action();
    }
}