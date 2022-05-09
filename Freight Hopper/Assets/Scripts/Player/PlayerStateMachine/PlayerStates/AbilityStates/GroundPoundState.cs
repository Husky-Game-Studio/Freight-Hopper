using System.Collections.Generic;
using System;

public class GroundPoundState : PlayerState
{
    public GroundPoundState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EntryState()
    {
        playerMachineCenter.groundPoundBehavior.GroundPoundInitialBurst();
        playerMachineCenter.groundPoundBehavior.EntryAction();
    }

    public override void ExitState()
    {
        playerMachineCenter.groundPoundBehavior.ExitAction();
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();

        return state;
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.friction.ReduceFriction(playerMachineCenter.groundPoundBehavior.FrictionReduction);
        playerMachineCenter.movementBehavior.MoveAction();
        playerMachineCenter.groundPoundBehavior.Action();
    }
}