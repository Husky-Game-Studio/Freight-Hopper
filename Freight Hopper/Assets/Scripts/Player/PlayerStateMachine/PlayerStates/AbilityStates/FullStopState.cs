using System.Collections.Generic;
using System;

public class FullStopState : PlayerState
{
    public FullStopState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EnterState()
    {
        playerMachineCenter.abilities.fullstopBehavior.EntryAction();
    }

    public override void ExitState()
    {
        playerMachineCenter.abilities.fullstopBehavior.ExitAction();
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

        return this;
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.abilities.fullstopBehavior.Action();
    }
}