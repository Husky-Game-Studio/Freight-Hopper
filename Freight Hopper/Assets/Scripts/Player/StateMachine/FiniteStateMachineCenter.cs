using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FiniteStateMachineCenter : MonoBehaviour
{
    // Machine Fields
    [SerializeField] private string currentStateName;
    [SerializeField] private string currentSubStateName;
    public BasicState currentState;
    public BasicState previousState;
    public BasicState currentSubState;

    // MonoBehavior functions
    public abstract void OnValidate();
    public abstract void OnEnable();
    public abstract void OnDisable();

    // Pefromed each LateFixedUpdate
    public void LateFixedUpdate() {
        // perform anything that is independent of being in any one single state
        this.performStateIndependentBehaviors();

        // checks if the prevousState is not the currentState
        this.checkAndChangeCurrentState();

        // Perform state behavior
        currentState.PerformBehavior(this);

        // check if the state needs to transition, and return the state it should belong in
        currentState = currentState.TransitionState(this);

        // Debugging
        currentStateName = currentState.ToString();
        if (!currentState.HasSubStateMachine())
        {
            currentSubStateName = "No Sub States";
        }
        else
        {
            currentSubState = currentState.GetCurrentSubState();
            currentSubStateName = currentSubState.ToString();
        }
    }

    // perform anything that is independent of being in any one single state
    public abstract void performStateIndependentBehaviors();

    // checks if the prevousState is not the currentState
    private void checkAndChangeCurrentState()
    {
        // If current state is a new transisiton, unsub from old listeners, and sub to new ones
        if (previousState != currentState)
        {
            currentState.SubToListeners(this);
            previousState.UnsubToListeners(this);
            previousState = currentState;
        }
    }

    // Returns currentState field
    public BasicState GetCurrentState() { return currentState; }
    public BasicState GetPreviousState() { return previousState; }
}

