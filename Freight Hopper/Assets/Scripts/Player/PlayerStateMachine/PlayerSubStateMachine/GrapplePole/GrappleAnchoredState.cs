using System.Collections.Generic;
using System;

public class GrappleAnchoredState : PlayerState
{
    public GrappleAnchoredState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override BasicState TransitionState()
    {
        return this;
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.abilities.grapplePoleBehavior.Grapple(UserInput.Instance.Move());
    }
}