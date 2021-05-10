using System.Collections.Generic;
using System;

public class IdleState : PlayerState
{
    public IdleState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EnterState()
    {
        playerMachineCenter.abilities.jumpBehavior.coyoteeTimer.DeactivateTimer();
        foreach (AbilityBehavior ability in playerMachineCenter.abilities.Abilities)
        {
            ability.Recharge();
        }
    }

    public override void ExitState()
    {
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
    }
}