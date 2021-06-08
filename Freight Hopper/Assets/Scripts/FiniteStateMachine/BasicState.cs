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

    public BasicState(FiniteStateMachineCenter machineCenter)
    {
        this.machineCenter = machineCenter;
        this.stateTransitions = null;
    }

    public virtual void EntryState()
    {
    }

    public virtual void ExitState()
    {
    }

    protected BasicState CheckTransitions()
    {
        if (this.stateTransitions == null) {
            return null;
        }
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