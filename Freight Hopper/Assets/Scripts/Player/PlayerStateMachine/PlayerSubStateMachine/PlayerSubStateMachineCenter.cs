using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSubStateMachineCenter : SubStateMachineCenter
{
    // inherented and parent fields
    /*private BasicState parentState;
    private PlayerMachineCenter playerMachine;*/
    private PlayerMovement playerMovement;
    private CollisionManagement collision;

    /*// substate fields
    public BasicState[] miniStatesArray;
    public bool completedSubStateBehavior;
    private BasicState currentState;
    private BasicState previousState;*/

    public PlayerSubStateMachineCenter(BasicState myParentState, BasicState[] myMiniStatesArray, PlayerMachineCenter myPlayerMachine)
    {
        machineCenter = myPlayerMachine;

        parentState = myParentState;
        //machineCenter = myPlayerMachine;
        miniStatesArray = myMiniStatesArray;
        completedSubStateBehavior = false;
        currentState = miniStatesArray[0];
        previousState = currentState;

        playerMovement = myPlayerMachine.playerMovement;
        collision = myPlayerMachine.collision;
    }

    /*public override void OnValidate()
    {
        //public UserInput userInput;
        
    }

    public override void OnEnable()
    {
        //currentState.SubToListeners(playerMachine);
    }

    public override void OnDisable()
    {
        //currentState.UnsubToListeners(playerMachine);
    }*/

    /*public void PerformSubMachineBehavior()
    {
        // If current state is a new transisiton, unsub from old listeners, and sub to new ones
        this.checkAndChangeCurrentState(playerMachine);

        // Perform state behavior
        currentState.PerformBehavior(playerMachine);

        // check if the state needs to transition, and return the state it should belong in
        currentState = currentState.TransitionState(playerMachine);
    }

    private void checkAndChangeCurrentState(PlayerMachineCenter playerMachine)
    {
        // If current state is a new transisiton, unsub from old listeners, and sub to new ones
        if (previousState != currentState)
        {
            currentState.SubToListeners(playerMachine);
            previousState.UnsubToListeners(playerMachine);
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
    }*/
}
