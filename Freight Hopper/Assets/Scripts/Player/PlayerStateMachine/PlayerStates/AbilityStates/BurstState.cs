using System;
using System.Collections.Generic;

public class BurstState : PlayerState
{
    public BurstState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
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
        return playerMachineCenter.defaultState;
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.abilities.movementBehavior.MoveAction();
        playerMachineCenter.abilities.burstBehavior.Action();
    }
}