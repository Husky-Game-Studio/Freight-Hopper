using System.Collections.Generic;
using System;

public class FallState : PlayerState
{
    public FallState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EnterState()
    {
        if (playerMachineCenter.GetPreviousState() != playerMachineCenter.jumpState)
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
        playerMachineCenter.pFSMTH.ResetInputs();
    }

    public override BasicState TransitionState()
    {
        foreach (Func<BasicState> stateCheck in this.stateTransitions)
        {
            BasicState tempState = stateCheck();
            if (tempState != null)
            {
                return tempState;
            }
        }

        playerMachineCenter.pFSMTH.ResetInputs();
        return this;
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