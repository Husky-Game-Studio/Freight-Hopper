using System.Collections.Generic;
using System;

public class GrappleFullstop : PlayerState
{
    public GrappleFullstop(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EnterState()
    {
        playerMachineCenter.abilities.fullstopBehavior.EntryAction();
    }

    public override void ExitState()
    {
        playerMachineCenter.abilities.fullstopBehavior.ExitAction();
    }

    public override BasicState TransitionState()
    {
        return this;
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.abilities.fullstopBehavior.Action();
    }
}