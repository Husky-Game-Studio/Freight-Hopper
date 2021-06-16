using System.Collections.Generic;
using System;

public class WanderState : BasicState
{
    public WanderState(FiniteStateMachineCenter machineCenter, List<Func<BasicState>> stateTransitions) : base(machineCenter, stateTransitions)
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