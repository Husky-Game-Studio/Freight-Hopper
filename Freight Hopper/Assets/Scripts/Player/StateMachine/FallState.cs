using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : BasicState
{
    public BasicState DoState(PlayerMachineCenter myPlayer)
    {
        /*if (myPlayer.playerMovement.)
        {
            Debug.Log("In Fall state!: " + myPlayer.playerMovement.getIsGrounded().current);
            return this;
        }*/
        //else
        //{
            Debug.Log("Should be in Idle state");
            return myPlayer.idleState;
        //}
    }
}
