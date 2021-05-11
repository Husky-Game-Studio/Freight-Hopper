using System.Collections.Generic;
using System;

public class DoubleJumpState : PlayerState
{
    public DoubleJumpState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EnterState()
    {
        // reset jump hold timer
        playerMachineCenter.abilities.jumpBehavior.jumpHoldingTimer.ResetTimer();
        playerMachineCenter.abilities.doubleJumpBehavior.EntryAction();
    }

    public override void ExitState()
    {
        // deactivate jump hold timer
        playerMachineCenter.abilities.jumpBehavior.jumpHoldingTimer.DeactivateTimer();
        playerMachineCenter.abilities.doubleJumpBehavior.ExitAction();
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
        playerMachineCenter.abilities.doubleJumpBehavior.Action();
    }
}