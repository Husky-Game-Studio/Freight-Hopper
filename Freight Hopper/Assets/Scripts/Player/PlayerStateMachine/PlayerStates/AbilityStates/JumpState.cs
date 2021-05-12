using System.Collections.Generic;
using System;

public class JumpState : PlayerState
{
    public JumpState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EntryState()
    {
        // reset jump hold timer
        playerMachineCenter.abilities.jumpBehavior.jumpHoldingTimer.ResetTimer();
        playerMachineCenter.abilities.jumpBehavior.EntryAction();
    }

    public override void ExitState()
    {
        // deactivate jump hold timer
        playerMachineCenter.abilities.jumpBehavior.jumpHoldingTimer.DeactivateTimer();

        playerMachineCenter.abilities.jumpBehavior.ExitAction();
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();

        return state;
    }

    public override void PerformBehavior()
    {
        // each fixedupdate the jump button is pressed down, this timer should decrease by that time
        playerMachineCenter.abilities.jumpBehavior.jumpHoldingTimer.CountDownFixed();
        playerMachineCenter.abilities.movementBehavior.PlayerMove();
        playerMachineCenter.abilities.jumpBehavior.Action();
    }
}