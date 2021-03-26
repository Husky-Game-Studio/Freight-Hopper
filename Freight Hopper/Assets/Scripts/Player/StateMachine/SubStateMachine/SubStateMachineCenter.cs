using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubStateMachineCenter
{
    // inherented and parent fields
    private BasicState parentState;
    private PlayerMachineCenter playerMachine;
    private PlayerMovement playerMovement;
    private CollisionManagement collision;

    // substate fields
    public BasicState[] miniStatesArray;
    public bool completedSubStateBehavior;
    private BasicState currentState;
    private BasicState previousState;

    public SubStateMachineCenter(BasicState myParentState, BasicState[] myMiniStatesArray, PlayerMachineCenter myPlayerMachine)
    {
        this.parentState = myParentState;
        this.playerMachine = myPlayerMachine;
        miniStatesArray = myMiniStatesArray;
        completedSubStateBehavior = false;
        currentState = miniStatesArray[0];
        previousState = currentState;

        playerMovement = this.playerMachine.playerMovement;
        collision = this.playerMachine.collision;
    }

    /*private void OnValidate()
    {
        //public UserInput userInput;
        
    }*/

    /*private void OnEnable()
    {
        currentState.SubToListeners(playerMachine);
    }

    void OnDisable()
    {
        currentState.UnsubToListeners(playerMachine);
    }*/

    public void PerformSubMachineBehavior()
    {
        // If current state is a new transisiton, unsub from old listeners, and sub to new ones
        if (previousState != currentState)
        {
            currentState.SubToListeners(playerMachine);
            previousState.UnsubToListeners(playerMachine);
            previousState = currentState;
        }

        // Perform state behavior
        currentState.PerformBehavior(playerMachine);

        // check if the state needs to transition, and return the state it should belong in
        currentState = currentState.TransitionState(playerMachine);
    }

    public BasicState GetCurrentSubState()
    {
        return this.currentState;
    }
}
