using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SubStateMachineCenter
{
    // inherented and parent fields
    public BasicState parentState;
    public FiniteStateMachineCenter machineCenter;

    // substate fields
    public BasicState[] miniStatesArray;
    public bool completedSubStateBehavior;
    public BasicState currentState;
    public BasicState previousState;

    // SubState Machine Functions
    public void PerformSubMachineBehavior()
    {
        // If current state is a new transisiton, unsub from old listeners, and sub to new ones
        this.CheckAndChangeCurrentStateListeners(machineCenter);

        // Perform state behavior
        currentState.PerformBehavior(machineCenter);

        // check if the state needs to transition, and return the state it should belong in
        currentState = currentState.TransitionState(machineCenter);
    }

    private void CheckAndChangeCurrentStateListeners(FiniteStateMachineCenter machineCenter)
    {
        // If current state is a new transisiton, unsub from old listeners, and sub to new ones
        if (previousState != currentState)
        {
            currentState.SubToListeners(machineCenter);
            previousState.UnsubToListeners(machineCenter);
            previousState = currentState;
        }
    }

    public BasicState GetCurrentSubState()
    {
        return this.currentState;
    }

    public void setPrevCurrState(BasicState subState)
    {
        currentState = subState;
        previousState = subState;
    }
}
