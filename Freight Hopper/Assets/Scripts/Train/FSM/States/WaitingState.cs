using System.Collections.Generic;
using System;

public class WaitingState : BasicState
{
    public WaitingState(FiniteStateMachineCenter machineCenter, List<Func<BasicState>> stateTransitions) : base(machineCenter, stateTransitions)
    {
    }

    public override void PerformBehavior()
    {
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();
        return state;
    }
}