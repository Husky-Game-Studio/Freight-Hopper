using System.Collections.Generic;
using System;

public class GrappleGroundPoundState : PlayerState
{
    public GrappleGroundPoundState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EnterState()
    {
        playerMachineCenter.abilities.groundPoundBehavior.EntryAction();
    }

    public override void ExitState()
    {
        playerMachineCenter.abilities.groundPoundBehavior.ExitAction();
    }

    public override BasicState TransitionState()
    {
        return CheckTransitions();
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.abilities.groundPoundBehavior.Action();
    }
}