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
        playerMachineCenter.jumpBehavior.jumpHoldingTimer.ResetTimer();
        playerMachineCenter.jumpBehavior.coyoteeTimer.DeactivateTimer();
        playerMachineCenter.jumpBehavior.EntryAction();
    }

    public override void ExitState()
    {
        // deactivate jump hold timer
        playerMachineCenter.jumpBehavior.jumpHoldingTimer.DeactivateTimer();

        playerMachineCenter.jumpBehavior.ExitAction();
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();

        return state;
    }

    public override void PerformBehavior()
    {
        // each fixedupdate the jump button is pressed down, this timer should decrease by that time
        playerMachineCenter.jumpBehavior.jumpHoldingTimer.CountDown(UnityEngine.Time.fixedDeltaTime);
        if (!playerMachineCenter.collisionManagement.IsGrounded.current)
        {
            playerMachineCenter.jumpBehavior.coyoteeTimer.DeactivateTimer();
        }
        playerMachineCenter.movementBehavior.MoveAction();
        playerMachineCenter.jumpBehavior.Action();
    }
}