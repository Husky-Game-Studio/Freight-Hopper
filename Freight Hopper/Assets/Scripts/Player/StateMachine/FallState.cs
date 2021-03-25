using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : BasicState
{
    private bool playerLanded = false;

    public void SubToListeners(PlayerMachineCenter playerMachine)
    {
        // sub to Landed to trigger a function that returns a bool. and use that to pass or fail the if checks
        playerMachine.collision.Landed += this.HasLanded;
    }

    public void UnsubToListeners(PlayerMachineCenter playerMachine) {
        playerMachine.collision.Landed -= this.HasLanded;
    }

    public BasicState TransitionState(PlayerMachineCenter playerMachine)
    {
        

        // Fall
        if (!playerLanded)
        {
            //Debug.Log("In Fall state!: " + myPlayer.playerMovement.getIsGrounded().old);
            return this;
        }
        // Idle
        else
        {
            playerLanded = false;
            //Debug.Log("Should be in Idle state");
            return playerMachine.idleState;
        }
    }

    public void PerformBehavior(PlayerMachineCenter playerMachine)
    {
        playerMachine.playerMovement.movement.Movement();
    }


    private void HasLanded() {
        playerLanded = true;
    }
}
