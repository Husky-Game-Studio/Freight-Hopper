using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSubStateMachineCenter : SubStateMachineCenter
{
    private PlayerMovement playerMovement;
    private CollisionManagement collision;

    public PlayerSubStateMachineCenter(BasicState myParentState, BasicState[] myMiniStatesArray, PlayerMachineCenter myPlayerMachine)
    {
        machineCenter = myPlayerMachine;

        parentState = myParentState;
        miniStatesArray = myMiniStatesArray;
        completedSubStateBehavior = false;
        currentState = miniStatesArray[0];
        previousState = currentState;

        playerMovement = myPlayerMachine.playerMovement;
        collision = myPlayerMachine.collision;
    }
}
