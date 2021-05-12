using System.Collections.Generic;
using System;

public class GroundPoundState : PlayerState
{
    public GroundPoundState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EntryState()
    {
        playerMachineCenter.abilities.groundPoundBehavior.EntryAction();
    }

    public override void ExitState()
    {
        playerMachineCenter.abilities.groundPoundBehavior.ExitAction();
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();

        return state;
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.abilities.movementBehavior.PlayerMove();
        playerMachineCenter.abilities.groundPoundBehavior.Action();
    }
}