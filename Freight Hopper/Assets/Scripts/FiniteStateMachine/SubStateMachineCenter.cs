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
        // Perform state behavior
        currentState.PerformBehavior();
        // check if the state needs to transition, and return the state it should belong in
        currentState = currentState.TransitionState();

        // If current state is a new transisiton, unsub from old listeners, and sub to new ones
        this.CheckAndChangeCurrentStateListeners();
    }

    private void CheckAndChangeCurrentStateListeners()
    {
        // If current state is a new transisiton, unsub from old listeners, and sub to new ones
        if (previousState != currentState)
        {
            currentState.EntryState();
            previousState.ExitState();
            previousState = currentState;
        }
    }

    public BasicState GetCurrentSubState()
    {
        return this.currentState;
    }

    public void SetPrevCurrState(BasicState subState)
    {
        currentState = subState;
        previousState = subState;
    }
}