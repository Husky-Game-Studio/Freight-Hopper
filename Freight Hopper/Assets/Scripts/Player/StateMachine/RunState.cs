using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : BasicState
{
    public BasicState DoState(PlayerMachineCenter myPlayer)
    {
        if (myPlayer.userInput.Move() != Vector2.zero)
        {
            //Debug.Log("in Run state!");
            return this;
        }
        else
        {
            //Debug.Log("Should be in Idle state");
            return myPlayer.idleState;
        }
    }

}
