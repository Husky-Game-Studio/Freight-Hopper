using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : BasicState
{
    public BasicState DoState(PlayerMachineCenter myPlayer)
    {
        if (myPlayer.userInput.Jump())
        {
            Debug.Log("In Jump state!");
            return this;
        }
        else
        {
            Debug.Log("Should be in Fall state");
            return myPlayer.fallState;
        }
    }
}
