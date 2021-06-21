using System.Collections.Generic;
using System;

public class FullStopState : PlayerState
{
    public FullStopState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EntryState()
    {
        playerMachineCenter.abilities.fullstopBehavior.EntryAction();
    }

    public override void ExitState()
    {
        playerMachineCenter.abilities.fullstopBehavior.ExitAction();
    }

    public override BasicState TransitionState()
    {
        return CheckTransitions();
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.abilities.fullstopBehavior.Action();
    }
}