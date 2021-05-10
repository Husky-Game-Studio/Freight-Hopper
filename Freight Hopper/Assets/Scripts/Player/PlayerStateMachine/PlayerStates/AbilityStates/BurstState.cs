using System;
using System.Collections.Generic;

public class BurstState : PlayerState
{
    public BurstState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EnterState()
    {
        playerMachineCenter.abilities.burstBehavior.EntryAction();
    }

    public override void ExitState()
    {
        playerMachineCenter.abilities.burstBehavior.ExitAction();
    }

    public override BasicState TransitionState()
    {
        return playerMachineCenter.fallState;
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.abilities.burstBehavior.Action();
    }
}