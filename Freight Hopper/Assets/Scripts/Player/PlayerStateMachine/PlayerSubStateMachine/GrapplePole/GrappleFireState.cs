using System.Collections.Generic;
using System;

public class GrappleFireState : PlayerState
{
    public GrappleFireState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EnterState()
    {
        playerMachineCenter.abilities.grapplePoleBehavior.EntryAction();
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
        playerMachineCenter.abilities.movementBehavior.PlayerMove();
        playerMachineCenter.abilities.grapplePoleBehavior.GrappleTransition();
    }
}