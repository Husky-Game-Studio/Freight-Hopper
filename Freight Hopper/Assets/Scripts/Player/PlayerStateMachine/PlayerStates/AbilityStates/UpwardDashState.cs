using System.Collections.Generic;
using System;

public class UpwardDashState : PlayerState
{
    public UpwardDashState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EnterState()
    {
        playerMachineCenter.abilities.upwardDashBehavior.EntryAction();
    }

    public override void ExitState()
    {
        playerMachineCenter.abilities.upwardDashBehavior.ExitAction();
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
        playerMachineCenter.abilities.upwardDashBehavior.Action();
    }
}