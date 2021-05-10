using System.Collections.Generic;
using System;

// State Machine help from these url:
// https://www.youtube.com/watch?v=nnrOhb5UdRc

public abstract class BasicState
{
    protected List<Func<BasicState>> stateTransitions;
    protected FiniteStateMachineCenter machineCenter;

    public BasicState(FiniteStateMachineCenter machineCenter, List<Func<BasicState>> stateTransitions)
    {
        this.machineCenter = machineCenter;
        this.stateTransitions = stateTransitions;
    }

    public virtual void EnterState()
    {
    }

    public virtual void ExitState()
    {
    }

    public abstract BasicState TransitionState();

    public abstract void PerformBehavior();

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