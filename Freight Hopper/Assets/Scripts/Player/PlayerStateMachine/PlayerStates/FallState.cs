using System.Collections.Generic;
using System;

public class FallState : PlayerState
{
    public FallState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EntryState()
    {
    }

    public override void ExitState()
    {
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();

        return state;
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.abilities.movementBehavior.PlayerMove();
    }
}