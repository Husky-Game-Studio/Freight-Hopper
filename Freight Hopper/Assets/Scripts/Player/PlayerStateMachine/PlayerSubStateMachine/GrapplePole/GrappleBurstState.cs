using System.Collections.Generic;
using System;

public class GrappleBurstState : PlayerState
{
    public GrappleBurstState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EntryState()
    {
        playerMachineCenter.abilities.burstBehavior.EntryAction();
    }

    public override void ExitState()
    {
        playerMachineCenter.abilities.burstBehavior.ExitAction();
    }

    public override BasicState TransitionState()
    {
        return playerMachineCenter.grapplePoleAnchoredState;
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.abilities.burstBehavior.Action();
    }
}