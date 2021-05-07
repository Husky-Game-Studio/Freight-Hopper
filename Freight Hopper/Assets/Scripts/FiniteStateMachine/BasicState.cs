using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// State Machine help from these url:
// https://www.youtube.com/watch?v=nnrOhb5UdRc

public abstract class BasicState
{
    protected List<Func<BasicState>> stateTransitions;

    public virtual void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
    }

    public virtual void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
    }

    public abstract BasicState TransitionState(FiniteStateMachineCenter machineCenter);

    public abstract void PerformBehavior(FiniteStateMachineCenter machineCenter);

    public virtual bool HasSubStateMachine()
    {
        return false;
    }

    public virtual BasicState GetCurrentSubState()
    {
        return null;
    }

    public virtual BasicState[] GetSubStateArray()
    {
        return null;
    }

    public List<Func<BasicState>> GetStateTransitions()
    {
        return stateTransitions;
    }
}