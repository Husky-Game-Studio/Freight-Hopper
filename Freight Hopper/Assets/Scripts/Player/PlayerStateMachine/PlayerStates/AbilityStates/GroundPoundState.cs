using System.Collections.Generic;
using System;

public class GroundPoundState : PlayerState
{
    public GroundPoundState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EnterState()
    {
        playerMachineCenter.abilities.groundPoundBehavior.EntryAction();
    }

    public override void ExitState()
    {
        playerMachineCenter.abilities.groundPoundBehavior.ExitAction();
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
        playerMachineCenter.abilities.groundPoundBehavior.Action();
    }
}