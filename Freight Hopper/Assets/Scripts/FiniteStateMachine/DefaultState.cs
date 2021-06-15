using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultState : BasicState
{
    public DefaultState(FiniteStateMachineCenter turretMachineCenter) : base(turretMachineCenter)
    {
    }

    public override BasicState TransitionState()
    {
        return this;
    }

    public override void PerformBehavior()
    {
    }
}