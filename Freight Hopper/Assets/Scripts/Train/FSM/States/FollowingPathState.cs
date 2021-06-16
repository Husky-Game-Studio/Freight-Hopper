using System;
using System.Collections.Generic;

public class FollowingPathState : BasicState
{
    public FollowingPathState(FiniteStateMachineCenter machineCenter, List<Func<BasicState>> stateTransitions) : base(machineCenter, stateTransitions)
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