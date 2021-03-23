using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BasicState
{
    public BasicState DoState(PlayerMachineCenter myPlayer) {
        // if the run buttons was pressed, then go to run state (return myPlayer.runState)
        if (myPlayer.userInput.Move() != Vector2.zero)
        {
            //Debug.Log("Should be in Run state!");
            return myPlayer.runState;
        }
        else {
            //Debug.Log("In Idle state");
            return this;
        }

        // if the jump button was pressed, then go to jump state (return myPlayer.jumpState)

        // if the <INSERT ABILITY> button was pressed, then go to <INSERT ABILITY> state (return myPlayer.<INSERT ABILITY>State)
        

    }
}
