using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// State Machine help from these url:
// https://www.youtube.com/watch?v=nnrOhb5UdRc

public interface BasicState
{
    public void SubToListeners(FiniteStateMachineCenter machineCenter);

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter);

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter);

    public void PerformBehavior(FiniteStateMachineCenter machineCenter);

    public bool HasSubStateMachine();

    public BasicState GetCurrentSubState();

    public BasicState[] GetSubStateArray();
}