using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

// Help developing this and related code using this YouTube video: https://www.youtube.com/watch?v=V75hgcsCGOM&t=944s
// Addition help from this link on delagate funcs: https://www.tutorialsteacher.com/csharp/csharp-func-delegate
public class PlayerStatesTransitions
{
    private PlayerMachineCenter playerMachine;

    public Toggle jumpPressed = new Toggle();
    public Toggle jumpReleased = new Toggle();
    public Toggle groundPoundPressed = new Toggle();
    public Toggle groundPoundReleased = new Toggle();
    private bool lastStateGroundPound = false;

    public void ResetInputs()
    {
        jumpPressed.Reset();
        jumpReleased.Reset();
        groundPoundPressed.Reset();
        groundPoundReleased.Reset();
    }

    public void OnDisable()
    {
        this.UnsubToListeners();
    }

    private void SubToListeners()
    {
        UserInput.Instance.JumpInput += jumpPressed.Trigger;
        UserInput.Instance.JumpInputCanceled += jumpReleased.Trigger;
        UserInput.Instance.GroundPoundInput += groundPoundPressed.Trigger;
        UserInput.Instance.GroundPoundCanceled += groundPoundReleased.Trigger;
    }

    private void UnsubToListeners()
    {
        UserInput.Instance.JumpInput -= jumpPressed.Trigger;
        UserInput.Instance.JumpInputCanceled -= jumpReleased.Trigger;
        UserInput.Instance.GroundPoundInput -= groundPoundPressed.Trigger;
        UserInput.Instance.GroundPoundCanceled -= groundPoundReleased.Trigger;
    }

    public PlayerStatesTransitions(FiniteStateMachineCenter machineCenter)
    {
        SubToListeners();

        playerMachine = (PlayerMachineCenter)machineCenter;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // The conditional functions to check if the PFSM should transition to a different state ///////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public BasicState CheckToDefaultState()
    {
        if (!UserInput.Instance.GroundPoundHeld || playerMachine.currentState != playerMachine.jumpState)
        {
            lastStateGroundPound = false;
        }
        if (playerMachine.currentState == playerMachine.moveState && UserInput.Instance.Move() == Vector3.zero)
        {
            return playerMachine.defaultState;
        }

        // Jump
        if ((jumpReleased.value || !playerMachine.jumpBehavior.jumpHoldingTimer.TimerActive()) &&
            playerMachine.currentState == playerMachine.jumpState)
        {
            return playerMachine.defaultState;
        }

        // Ground Pound
        if ((groundPoundReleased.value && playerMachine.currentState == playerMachine.groundPoundState) ||
            (playerMachine.groundPoundBehavior.FlatSurface && playerMachine.currentState == playerMachine.groundPoundState))
        {
            return playerMachine.defaultState;
        }


        // Wall run ground fail
        if (playerMachine.collisionManagement.IsGrounded.current && playerMachine.currentState == playerMachine.wallRunState)
        {
            return playerMachine.defaultState;
        }

        // Wall run
        if (playerMachine.currentState == playerMachine.wallRunState)
        {
            IList<bool> status = playerMachine.wallRunBehavior.WallStatus;
            // Fall from wall climb
            if ((!status[0] && !status[1] && !status[2] &&
                playerMachine.wallRunState.GetPlayerSubStateMachineCenter().currentState != playerMachine.wallRunState.GetSubStateArray()[2] &&
                playerMachine.wallRunState.GetPlayerSubStateMachineCenter().currentState != playerMachine.wallRunState.GetSubStateArray()[1]
                ) || UserInput.Instance.Move().z != 1)
            {
                playerMachine.wallRunBehavior.coyoteTimer.CountDown(Time.fixedDeltaTime);
                if (!playerMachine.wallRunBehavior.coyoteTimer.TimerActive())
                {
                    playerMachine.wallRunBehavior.coyoteTimer.ResetTimer();
                    return playerMachine.defaultState;
                }
            }
            if (playerMachine.wallRunState.GetPlayerSubStateMachineCenter().currentState == playerMachine.wallRunState.GetSubStateArray()[2]
                && (jumpReleased.value || !playerMachine.wallRunBehavior.jumpHoldingTimer.TimerActive()))
            {
                return playerMachine.defaultState;
            }
            if (playerMachine.wallRunState.GetPlayerSubStateMachineCenter().currentState == playerMachine.wallRunState.GetSubStateArray()[1] && !status[1])
            {
                return playerMachine.defaultState;
            }
        }

        return null;
    }

    public BasicState CheckToJumpState()
    {
        // Ground Pound
        if ((jumpPressed.value || UserInput.Instance.JumpHeld) &&
            (playerMachine.jumpBehavior.coyoteeTimer.TimerActive() || playerMachine.collisionManagement.IsGrounded.current) &&
            playerMachine.currentState == playerMachine.groundPoundState)
        {
            lastStateGroundPound = true;
            return playerMachine.jumpState;
        }

        // Other
        if ((jumpPressed.value || UserInput.Instance.JumpHeld) &&
            (playerMachine.jumpBehavior.coyoteeTimer.TimerActive() || playerMachine.collisionManagement.IsGrounded.current))
        {
            return playerMachine.jumpState;
        }

        if (jumpPressed.value && playerMachine.jumpBehavior.Consumed)
        {
            playerMachine.jumpBehavior.PlayerSoundManager().Play("JumpFail");
        }

        return null;
    }

    public BasicState CheckToMoveState()
    {
        if (playerMachine.currentState == playerMachine.defaultState &&
            UserInput.Instance.Move() != Vector3.zero)
        {
            return playerMachine.moveState;
        }
        return null;
    }

    public BasicState CheckToGroundPoundState()
    {
        if (UserInput.Instance.GroundPoundHeld &&
            !playerMachine.groundPoundBehavior.FlatSurface && !lastStateGroundPound)
        {
            return playerMachine.groundPoundState;
        }

        return null;
    }

 

    public BasicState CheckToWallRunState()
    {
        if (!playerMachine.wallRunBehavior.inAirCooldown.TimerActive())
        {
            IList<bool> walls = playerMachine.wallRunBehavior.WallStatus;
            if (walls[0] || walls[1] || walls[2])
            {
                return playerMachine.wallRunState;
            }
        }

        return null;
    }

    public BasicState CheckToWallRunWallClimbingState()
    {
        IList<bool> status = playerMachine.wallRunBehavior.WallStatus;
        // Wall Climb
        if (status[1] && !status[0] && !status[2] && UserInput.Instance.Move().z == 1)
        {
            return playerMachine.GetCurrentState().GetSubStateArray()[1];
        }
        return null;
    }

    public BasicState CheckToWallRunWallJumpState()
    {
        if (jumpPressed.value)
        {
            return playerMachine.GetCurrentState().GetSubStateArray()[2];
        }
        return null;
    }
}