using System.Collections.Generic;
using System;

public class FallState : PlayerState
{
    public FallState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EntryState()
    {
        if (playerMachineCenter.GetPreviousState() == playerMachineCenter.idleState || playerMachineCenter.GetPreviousState() == playerMachineCenter.runState)
        {
            playerMachineCenter.abilities.jumpBehavior.coyoteeTimer.ResetTimer();
        }
        else
        {
            playerMachineCenter.abilities.jumpBehavior.coyoteeTimer.DeactivateTimer();
        }
    }

    public override void ExitState()
    {
        playerMachineCenter.abilities.jumpBehavior.coyoteeTimer.DeactivateTimer();
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();

        return state;
    }

    public override void PerformBehavior()
    {
        if (playerMachineCenter.GetPreviousState() != playerMachineCenter.jumpState)
        {
            playerMachineCenter.abilities.jumpBehavior.coyoteeTimer.CountDown();
        }

        playerMachineCenter.abilities.movementBehavior.PlayerMove();
    }
}