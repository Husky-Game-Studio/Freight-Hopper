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
    public Toggle grapplePressed = new Toggle();
    public Toggle grappleReleased = new Toggle();
    public Toggle groundPoundPressed = new Toggle();
    public Toggle groundPoundReleased = new Toggle();
    public Toggle upwardDashPressed = new Toggle();
    public Toggle fullStopPressed = new Toggle();
    public Toggle burstPressed = new Toggle();
    public Toggle releasedUpwardDash = new Toggle();

    public void ResetInputs()
    {
        jumpPressed.Reset();
        jumpReleased.Reset();
        grapplePressed.Reset();
        grappleReleased.Reset();
        groundPoundPressed.Reset();
        groundPoundReleased.Reset();
        upwardDashPressed.Reset();
        fullStopPressed.Reset();
        burstPressed.Reset();
        releasedUpwardDash.Reset();
    }

    public void OnDisable()
    {
        this.UnsubToListeners();
    }

    private void SubToListeners()
    {
        UserInput.Instance.JumpInput += jumpPressed.Trigger;
        UserInput.Instance.JumpInputCanceled += jumpReleased.Trigger;
        UserInput.Instance.GrappleInput += grapplePressed.Trigger;
        UserInput.Instance.GrappleInputCanceled += grappleReleased.Trigger;
        UserInput.Instance.GroundPoundInput += groundPoundPressed.Trigger;
        UserInput.Instance.GroundPoundCanceled += groundPoundReleased.Trigger;
        UserInput.Instance.UpwardDashInput += upwardDashPressed.Trigger;
        UserInput.Instance.FullStopInput += fullStopPressed.Trigger;
        UserInput.Instance.BurstInput += burstPressed.Trigger;
        UserInput.Instance.UpwardDashInputCanceled += releasedUpwardDash.Trigger;
    }

    private void UnsubToListeners()
    {
        UserInput.Instance.JumpInput -= jumpPressed.Trigger;
        UserInput.Instance.JumpInputCanceled -= jumpReleased.Trigger;
        UserInput.Instance.GrappleInput -= grapplePressed.Trigger;
        UserInput.Instance.GrappleInputCanceled -= grappleReleased.Trigger;
        UserInput.Instance.GroundPoundInput -= groundPoundPressed.Trigger;
        UserInput.Instance.GroundPoundCanceled -= groundPoundReleased.Trigger;
        UserInput.Instance.UpwardDashInput -= upwardDashPressed.Trigger;
        UserInput.Instance.FullStopInput -= fullStopPressed.Trigger;
        UserInput.Instance.BurstInput -= burstPressed.Trigger;
        UserInput.Instance.UpwardDashInputCanceled -= releasedUpwardDash.Trigger;
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
        if (playerMachine.currentState == playerMachine.moveState && UserInput.Instance.Move() == Vector3.zero)
        {
            return playerMachine.defaultState;
        }

        // Jump or Double Jump
        if ((jumpReleased.value || !playerMachine.abilities.jumpBehavior.jumpHoldingTimer.TimerActive()) &&
            (playerMachine.currentState == playerMachine.jumpState || playerMachine.currentState == playerMachine.doubleJumpState))
        {
            if (playerMachine.currentState == playerMachine.doubleJumpState)
            {
                playerMachine.abilities.jumpBehavior.jumpHoldingTimer.DeactivateTimer();
            }
            return playerMachine.defaultState;
        }

        // Upward Dash
        if ((releasedUpwardDash.value || !playerMachine.abilities.upwardDashBehavior.duration.TimerActive()) && playerMachine.currentState == playerMachine.upwardDashState)
        {
            return playerMachine.defaultState;
        }

        // Ground Pound
        if ((groundPoundReleased.value && playerMachine.currentState == playerMachine.groundPoundState) ||
            (playerMachine.abilities.groundPoundBehavior.FlatSurface && playerMachine.currentState == playerMachine.groundPoundState))
        {
            return playerMachine.defaultState;
        }

        // Grapple
        if ((burstPressed.value || playerMachine.abilities.grapplePoleBehavior.GrapplePoleBroken()) &&
            playerMachine.abilities.grapplePoleBehavior.IsAnchored() &&
            (playerMachine.currentState == playerMachine.grapplePoleAnchoredState))
        {
            return playerMachine.defaultState;
        }

        // Full stop
        if (playerMachine.abilities.fullstopBehavior.FullStopFinished() && (playerMachine.currentState == playerMachine.fullStopState))
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
            bool[] status = playerMachine.abilities.wallRunBehavior.WallStatus();
            // Fall from wall climb
            if ((!status[0] && !status[1] && !status[2] &&
                playerMachine.wallRunState.GetPlayerSubStateMachineCenter().currentState != playerMachine.wallRunState.GetSubStateArray()[2] &&
                playerMachine.wallRunState.GetPlayerSubStateMachineCenter().currentState != playerMachine.wallRunState.GetSubStateArray()[1]
                ) || UserInput.Instance.Move().z != 1)
            {
                playerMachine.abilities.wallRunBehavior.coyoteTimer.CountDownFixed();
                if (!playerMachine.abilities.wallRunBehavior.coyoteTimer.TimerActive())
                {
                    playerMachine.abilities.wallRunBehavior.coyoteTimer.ResetTimer();
                    return playerMachine.defaultState;
                }
            }
            if (playerMachine.wallRunState.GetPlayerSubStateMachineCenter().currentState == playerMachine.wallRunState.GetSubStateArray()[2]
                && (jumpReleased.value || !playerMachine.abilities.wallRunBehavior.jumpHoldingTimer.TimerActive()))
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
            playerMachine.abilities.jumpBehavior.Unlocked &&
            (playerMachine.abilities.jumpBehavior.coyoteeTimer.TimerActive() || playerMachine.collisionManagement.IsGrounded.current) &&
            playerMachine.currentState == playerMachine.groundPoundState)
        {
            return playerMachine.jumpState;
        }

        // Grapple Pole
        if (jumpPressed.value &&
            playerMachine.currentState == playerMachine.grapplePoleAnchoredState)
        {
            return playerMachine.jumpState;
        }

        // Other
        if ((jumpPressed.value || UserInput.Instance.JumpHeld) &&
            playerMachine.abilities.jumpBehavior.Unlocked &&
            (playerMachine.abilities.jumpBehavior.coyoteeTimer.TimerActive() || playerMachine.collisionManagement.IsGrounded.current))
        {
            return playerMachine.jumpState;
        }

        if (jumpPressed.value &&
            playerMachine.abilities.jumpBehavior.Unlocked && playerMachine.abilities.jumpBehavior.Consumed &&
            !playerMachine.abilities.doubleJumpBehavior.Unlocked)
        {
            playerMachine.abilities.jumpBehavior.PlayerSoundManager().Play("JumpFail");
        }

        return null;
    }

    public BasicState CheckToMoveState()
    {
        if (playerMachine.currentState == playerMachine.defaultState &&
            UserInput.Instance.Move() != Vector3.zero && playerMachine.abilities.movementBehavior.Unlocked)
        {
            return playerMachine.moveState;
        }
        return null;
    }

    public BasicState CheckToDoubleJumpState()
    {
        // Other
        if (jumpPressed.value && !playerMachine.abilities.jumpBehavior.coyoteeTimer.TimerActive() && playerMachine.abilities.doubleJumpBehavior.UnlockedAndReady)
        {
            return playerMachine.doubleJumpState;
        }

        if (jumpPressed.value && playerMachine.abilities.doubleJumpBehavior.Unlocked && playerMachine.abilities.doubleJumpBehavior.Consumed)
        {
            playerMachine.abilities.doubleJumpBehavior.PlayerSoundManager().Play("JumpFail");
        }
        return null;
    }

    public BasicState CheckToGroundPoundState()
    {
        if (UserInput.Instance.GroundPoundHeld &&
            !playerMachine.abilities.groundPoundBehavior.FlatSurface &&
            playerMachine.abilities.groundPoundBehavior.Unlocked)
        {
            return playerMachine.groundPoundState;
        }

        return null;
    }

    public BasicState CheckToGrappleGroundPoundState()
    {
        if ((groundPoundPressed.value && playerMachine.currentState == playerMachine.grapplePoleAnchoredState) ||
            (grapplePressed.value && playerMachine.currentState == playerMachine.groundPoundState))
        {
            if ((playerMachine.collisionManagement.ContactNormal.current != playerMachine.collisionManagement.ValidUpAxis ||
                playerMachine.collisionManagement.IsGrounded.current == false) && playerMachine.abilities.groundPoundBehavior.Unlocked)
            {
                if (playerMachine.collisionManagement.IsGrounded.current)
                {
                    playerMachine.abilities.groundPoundBehavior.PreventConsumption();
                }

                return playerMachine.grapplePoleGroundPoundState;
            }
        }

        return null;
    }

    public BasicState CheckToUpwardDashState()
    {
        if (upwardDashPressed.value && playerMachine.abilities.upwardDashBehavior.UnlockedAndReady)
        {
            if (playerMachine.collisionManagement.IsGrounded.current)
            {
                playerMachine.abilities.upwardDashBehavior.PreventConsumption();
            }
            return playerMachine.upwardDashState;
        }

        if (upwardDashPressed.value && playerMachine.abilities.upwardDashBehavior.Unlocked && playerMachine.abilities.upwardDashBehavior.Consumed)
        {
            playerMachine.abilities.upwardDashBehavior.PlayerSoundManager().Play("UpwardDashFail");
        }
        return null;
    }

    public BasicState CheckToFullStopState()
    {
        if (fullStopPressed.value && playerMachine.abilities.fullstopBehavior.UnlockedAndReady)
        {
            if (playerMachine.collisionManagement.IsGrounded.current)
            {
                playerMachine.abilities.fullstopBehavior.PreventConsumption();
            }
            return playerMachine.fullStopState;
        }

        if (fullStopPressed.value && playerMachine.abilities.fullstopBehavior.Unlocked && playerMachine.abilities.fullstopBehavior.Consumed)
        {
            playerMachine.abilities.fullstopBehavior.PlayerSoundManager().Play("FullstopFail");
        }
        return null;
    }

    public BasicState CheckToGrappleFullStopState()
    {
        if (fullStopPressed.value && playerMachine.abilities.fullstopBehavior.UnlockedAndReady)
        {
            if (playerMachine.collisionManagement.IsGrounded.current)
            {
                playerMachine.abilities.fullstopBehavior.PreventConsumption();
            }
            return playerMachine.grapplePoleFullStopState;
        }

        if (fullStopPressed.value && playerMachine.abilities.fullstopBehavior.Unlocked && playerMachine.abilities.fullstopBehavior.Consumed)
        {
            playerMachine.abilities.fullstopBehavior.PlayerSoundManager().Play("FullstopFail");
        }
        return null;
    }

    public BasicState CheckToBurstState()
    {
        if (burstPressed.value && playerMachine.abilities.burstBehavior.UnlockedAndReady)
        {
            if (playerMachine.collisionManagement.IsGrounded.current)
            {
                playerMachine.abilities.burstBehavior.PreventConsumption();
            }
            return playerMachine.burstState;
        }
        if (burstPressed.value && playerMachine.abilities.burstBehavior.Unlocked && playerMachine.abilities.burstBehavior.Consumed)
        {
            playerMachine.abilities.burstBehavior.PlayerSoundManager().Play("BurstFail");
        }
        return null;
    }

    /* public BasicState CheckToGrappleBurstState()
     {
         if (burstPressed.value && playerMachine.abilities.burstBehavior.UnlockedAndReady)
         {
             if (playerMachine.collisionManagement.IsGrounded.current)
             {
                 playerMachine.abilities.burstBehavior.PreventConsumption();
             }
             return playerMachine.grapplePoleBurstState;
         }
         if (burstPressed.value && playerMachine.abilities.burstBehavior.Unlocked && playerMachine.abilities.burstBehavior.Consumed)
         {
             playerMachine.abilities.burstBehavior.PlayerSoundManager().Play("BurstFail");
         }
         return null;
     }*/

    public BasicState CheckToGrapplePoleAnchoredState()
    {
        if (playerMachine.abilities.grapplePoleBehavior.IsAnchored())
        {
            if (playerMachine.collisionManagement.IsGrounded.current)
            {
                playerMachine.abilities.grapplePoleBehavior.PreventConsumption();
            }

            if (playerMachine.currentState == playerMachine.grapplePoleFullStopState)
            {
                if (!playerMachine.abilities.fullstopBehavior.FullStopFinished())
                {
                    return null;
                }
            }

            if (playerMachine.currentState == playerMachine.grapplePoleGroundPoundState)
            {
                if (!groundPoundReleased.value)
                {
                    return null;
                }
            }
            return playerMachine.grapplePoleAnchoredState;
        }
        return null;
    }

    public BasicState CheckToWallRunState()
    {
        if (playerMachine.abilities.wallRunBehavior.Unlocked && !playerMachine.abilities.wallRunBehavior.inAirCooldown.TimerActive())
        {
            bool[] walls = playerMachine.abilities.wallRunBehavior.WallStatus();
            if (walls[0] || walls[1] || walls[2])
            {
                return playerMachine.wallRunState;
            }
        }

        return null;
    }

    public BasicState CheckToWallRunWallClimbingState()
    {
        bool[] status = playerMachine.abilities.wallRunBehavior.WallStatus();
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