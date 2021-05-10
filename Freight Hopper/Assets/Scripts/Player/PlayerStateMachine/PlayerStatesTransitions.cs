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

    // MOVE ALL THE SUBSCRIPTIONS FOR CHECKING INPUT FROM THE STATES TO HERE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    // Input Trackers //////////////////////////////////////////////////////////////////////////////////////////////////////////

    public bool jumpPressed = false;
    public bool grapplePressed = false;
    public bool groundPoundPressed = false;
    public bool upwardDashPressed = false;
    public bool fullStopPressed = false;
    public bool burstPressed = false;
    public bool releasedJumpPressed = false;
    public bool groundPoundReleased = false;
    public bool releasedUpwardDash = false;

    // Functions to to indicate user pressing inputs ///////////////////////////////////////////////////////////////////////////
    private void JumpButtonPressed()
    {
        jumpPressed = true;
    }

    private void GroundPoundButtonPressed()
    {
        groundPoundPressed = true;
    }

    private void UpwardDashPressed()
    {
        upwardDashPressed = true;
    }

    private void GrappleButtonPressed()
    {
        grapplePressed = true;
    }

    private void FullStopPressed()
    {
        fullStopPressed = true;
    }

    private void BurstPressed()
    {
        burstPressed = true;
    }

    public void ReleasedJumpButtonPressed()
    {
        releasedJumpPressed = true;
    }

    private void GroundPoundButtonReleased()
    {
        groundPoundReleased = true;
    }

    private void ReleasedUpwardDash()
    {
        releasedUpwardDash = true;
    }

    public void ResetInputs()
    {
        jumpPressed = false;
        grapplePressed = false;
        groundPoundPressed = false;
        upwardDashPressed = false;
        fullStopPressed = false;
        burstPressed = false;
        releasedJumpPressed = false;
        groundPoundReleased = false;
        releasedUpwardDash = false;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Delagates to store condition functions to transition to other states //////////////////////////////////////////////////

    public Func<BasicState> checkToIdleState;
    public Func<BasicState> checkToRunState;
    public Func<BasicState> checkToJumpState;
    public Func<BasicState> checkToFallState;
    public Func<BasicState> checkToDoubleJumpState;
    public Func<BasicState> checkToGroundPoundState;
    public Func<BasicState> checkToUpwardDashState;
    public Func<BasicState> checkToFullStopState;
    public Func<BasicState> checkToBurstState;
    public Func<BasicState> checkToGrapplePoleState;
    public Func<BasicState> checkToWallRunState;
    public Func<BasicState> checkToGrapplePoleAnchoredState;
    public Func<BasicState> checkToWallRunWallClimbingState;
    public Func<BasicState> checkToWallRunWallJumpState;

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnDisable()
    {
        this.UnsubToListeners();
    }

    private void SubToListeners()
    {
        UserInput.Instance.JumpInput += this.JumpButtonPressed;
        UserInput.Instance.JumpInputCanceled += this.ReleasedJumpButtonPressed;
        UserInput.Instance.GrappleInput += this.GrappleButtonPressed;
        UserInput.Instance.GroundPoundInput += this.GroundPoundButtonPressed;
        UserInput.Instance.GroundPoundCanceled += this.GroundPoundButtonReleased;
        UserInput.Instance.UpwardDashInput += this.UpwardDashPressed;
        UserInput.Instance.FullStopInput += this.FullStopPressed;
        UserInput.Instance.BurstInput += this.BurstPressed;
        UserInput.Instance.UpwardDashInputCanceled += this.ReleasedUpwardDash;
    }

    private void UnsubToListeners()
    {
        UserInput.Instance.JumpInput -= this.JumpButtonPressed;
        UserInput.Instance.JumpInputCanceled -= this.ReleasedJumpButtonPressed;
        UserInput.Instance.GrappleInput -= this.GrappleButtonPressed;
        UserInput.Instance.GroundPoundInput -= this.GroundPoundButtonPressed;
        UserInput.Instance.GroundPoundCanceled -= this.GroundPoundButtonReleased;
        UserInput.Instance.UpwardDashInput -= this.UpwardDashPressed;
        UserInput.Instance.FullStopInput -= this.FullStopPressed;
        UserInput.Instance.BurstInput -= this.BurstPressed;
        UserInput.Instance.UpwardDashInputCanceled -= this.ReleasedUpwardDash;
    }

    public PlayerStatesTransitions(FiniteStateMachineCenter machineCenter)
    {
        this.SubToListeners();

        playerMachine = (PlayerMachineCenter)machineCenter;

        checkToIdleState = ShouldTransisitonToIdleState;
        checkToRunState = ShouldTransitionToRunState;
        checkToJumpState = ShouldTransitionToJumpState;
        checkToFallState = ShouldTransitionToFallState;
        checkToDoubleJumpState = ShouldTransitionToDoubleJumpState;
        checkToGroundPoundState = ShouldTransitionToGroundPoundState;
        checkToUpwardDashState = ShouldTransitionToUpwardDashState;
        checkToFullStopState = ShouldTransitionToFullStopState;
        checkToBurstState = ShouldTransitionToBurstState;
        checkToGrapplePoleState = ShouldTransitionToGrapplePoleState;
        checkToWallRunState = ShouldTransitionToWallRunState;
        checkToGrapplePoleAnchoredState = ShouldTransitionToGrapplePoleAnchoredState;
        checkToWallRunWallClimbingState = ShouldTransistionToWallRunWallClimbingState;
        checkToWallRunWallJumpState = ShouldTransistionToWallRunWallJumpState;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // The conditional functions to check if the PFSM should transition to a different state ///////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Common transistion check to fall
    private BasicState ShouldTransitionToFallState()
    {
        if ((releasedJumpPressed || !playerMachine.abilities.jumpBehavior.jumpHoldingTimer.TimerActive()) &&
            (playerMachine.currentState == playerMachine.jumpState || playerMachine.currentState == playerMachine.doubleJumpState))
        {
            if (playerMachine.currentState == playerMachine.doubleJumpState)
            {
                playerMachine.abilities.jumpBehavior.jumpHoldingTimer.DeactivateTimer();
            }
            return playerMachine.fallState;
        }

        if ((!playerMachine.playerCM.IsGrounded.current) &&
            (playerMachine.currentState == playerMachine.runState || playerMachine.currentState == playerMachine.idleState))
        {
            return playerMachine.fallState;
        }

        if (releasedUpwardDash && playerMachine.currentState == playerMachine.upwardDashState)
        {
            return playerMachine.fallState;
        }

        if (groundPoundReleased && playerMachine.currentState == playerMachine.groundPoundState)
        {
            return playerMachine.fallState;
        }

        if ((grapplePressed || (playerMachine.abilities.grapplePoleBehavior.GrapplePoleBroken() &&
            playerMachine.abilities.grapplePoleBehavior.IsAnchored())) &&
            (playerMachine.currentState == playerMachine.grapplePoleState))
        {
            return playerMachine.fallState;
        }

        if ((playerMachine.abilities.fullstopBehavior.FullStopFinished()) && (playerMachine.currentState == playerMachine.fullStopState))
        {
            return playerMachine.fallState;
        }

        if (playerMachine.currentState == playerMachine.wallRunState)
        {
            bool[] status = playerMachine.abilities.wallRunBehavior.CheckWalls();
            // Fall from wall climb
            if (!status[0] && !status[1] && !status[3] &&
                playerMachine.wallRunState.GetPlayerSubStateMachineCenter().currentState != playerMachine.wallRunState.GetSubStateArray()[2] &&
                playerMachine.wallRunState.GetPlayerSubStateMachineCenter().currentState != playerMachine.wallRunState.GetSubStateArray()[1])
            {
                playerMachine.abilities.wallRunBehavior.coyoteTimer.CountDownFixed();
                if (!playerMachine.abilities.wallRunBehavior.coyoteTimer.TimerActive())
                {
                    playerMachine.abilities.wallRunBehavior.coyoteTimer.ResetTimer();
                    return playerMachine.fallState;
                }
            }
            if (playerMachine.wallRunState.GetPlayerSubStateMachineCenter().currentState == playerMachine.wallRunState.GetSubStateArray()[2]
                && (releasedJumpPressed || !playerMachine.abilities.wallRunBehavior.jumpHoldingTimer.TimerActive()))
            {
                return playerMachine.fallState;
            }
            if (playerMachine.wallRunState.GetPlayerSubStateMachineCenter().currentState == playerMachine.wallRunState.GetSubStateArray()[1] &&
                !playerMachine.abilities.wallRunBehavior.climbTimer.TimerActive())
            {
                return playerMachine.fallState;
            }
        }
        return null;
    }

    // Common transition check to jump
    private BasicState ShouldTransitionToJumpState()
    {
        if ((playerMachine.GetCurrentState() != playerMachine.fallState && (jumpPressed || playerMachine.abilities.jumpBehavior.jumpBufferTimer.TimerActive())
                    && (!playerMachine.abilities.jumpBehavior.Consumed && playerMachine.abilities.jumpBehavior.Unlocked)) ||
            (playerMachine.GetCurrentState() == playerMachine.fallState && (jumpPressed && playerMachine.abilities.jumpBehavior.coyoteeTimer.TimerActive()
                    && playerMachine.GetPreviousState() != playerMachine.jumpState && playerMachine.abilities.jumpBehavior.UnlockedAndReady))
        )
        {
            return playerMachine.jumpState;
        }
        if ((jumpPressed && playerMachine.abilities.jumpBehavior.UnlockedAndReady) &&
            (playerMachine.currentState == playerMachine.groundPoundState))
        {
            return playerMachine.jumpState;
        }
        if (jumpPressed && playerMachine.currentState == playerMachine.grapplePoleState)
        {
            return playerMachine.jumpState;
        }

        return null;
    }

    private BasicState ShouldTransisitonToIdleState()
    {
        if ((playerMachine.playerCM.IsGrounded.current && playerMachine.GetCurrentState() == playerMachine.fallState) ||
            (UserInput.Instance.Move() == Vector3.zero && playerMachine.GetCurrentState() == playerMachine.runState))
        {
            return playerMachine.idleState;
        }
        if (playerMachine.playerCM.IsGrounded.current && playerMachine.currentState == playerMachine.wallRunState)
        {
            return playerMachine.idleState;
        }
        return null;
    }

    private BasicState ShouldTransitionToRunState()
    {
        if (UserInput.Instance.Move() != Vector3.zero && playerMachine.abilities.movementBehavior.Unlocked)
        {
            return playerMachine.runState;
        }
        return null;
    }

    private BasicState ShouldTransitionToDoubleJumpState()
    {
        if (jumpPressed && playerMachine.abilities.doubleJumpBehavior.UnlockedAndReady &&
            playerMachine.currentState == playerMachine.fallState)
        {
            return playerMachine.doubleJumpState;
        }
        if (jumpPressed && playerMachine.abilities.jumpBehavior.Consumed &&
            playerMachine.abilities.doubleJumpBehavior.UnlockedAndReady &&
            playerMachine.currentState == playerMachine.doubleJumpState)
        {
            return playerMachine.doubleJumpState;
        }
        return null;
    }

    private BasicState ShouldTransitionToGroundPoundState()
    {
        if (groundPoundPressed &&
            (playerMachine.playerCM.ContactNormal.current != playerMachine.playerCM.ValidUpAxis ||
            playerMachine.playerCM.IsGrounded.current == false) && playerMachine.abilities.groundPoundBehavior.UnlockedAndReady)
        {
            if (playerMachine.currentState == playerMachine.idleState || playerMachine.currentState == playerMachine.runState)
            {
                playerMachine.abilities.groundPoundBehavior.PreventConsumption();
            }
            return playerMachine.groundPoundState;
        }

        return null;
    }

    private BasicState ShouldTransitionToUpwardDashState()
    {
        if (upwardDashPressed && playerMachine.abilities.upwardDashBehavior.UnlockedAndReady)
        {
            if (playerMachine.currentState == playerMachine.idleState || playerMachine.currentState == playerMachine.runState)
            {
                playerMachine.abilities.upwardDashBehavior.PreventConsumption();
            }
            return playerMachine.upwardDashState;
        }
        return null;
    }

    private BasicState ShouldTransitionToFullStopState()
    {
        if (fullStopPressed && playerMachine.abilities.fullstopBehavior.UnlockedAndReady)
        {
            return playerMachine.fullStopState;
        }
        return null;
    }

    private BasicState ShouldTransitionToBurstState()
    {
        if (burstPressed && playerMachine.abilities.burstBehavior.UnlockedAndReady)
        {
            if (playerMachine.currentState == playerMachine.idleState || playerMachine.currentState == playerMachine.runState)
            {
                playerMachine.abilities.burstBehavior.PreventConsumption();
            }
            return playerMachine.burstState;
        }
        return null;
    }

    private BasicState ShouldTransitionToGrapplePoleState()
    {
        if (grapplePressed && playerMachine.abilities.grapplePoleBehavior.UnlockedAndReady)
        {
            if (playerMachine.currentState == playerMachine.idleState || playerMachine.currentState == playerMachine.runState)
            {
                playerMachine.abilities.grapplePoleBehavior.PreventConsumption();
            }
            return playerMachine.grapplePoleState;
        }

        return null;
    }

    private BasicState ShouldTransitionToWallRunState()
    {
        if (playerMachine.abilities.wallRunBehavior.Unlocked)
        {
            bool[] walls = playerMachine.abilities.wallRunBehavior.CheckWalls();
            if (walls[0] || walls[1] || walls[3])
            {
                return playerMachine.wallRunState;
            }
        }

        return null;
    }

    private BasicState ShouldTransitionToGrapplePoleAnchoredState()
    {
        if (playerMachine.abilities.grapplePoleBehavior.IsAnchored())
        {
            return playerMachine.GetCurrentState().GetSubStateArray()[1];
        }
        return null;
    }

    private BasicState ShouldTransistionToWallRunWallClimbingState()
    {
        bool[] status = playerMachine.abilities.wallRunBehavior.CheckWalls();
        // Wall Climb
        if (status[0] && !status[1] && !status[3] && !playerMachine.abilities.wallRunBehavior.Consumed)
        {
            return playerMachine.GetCurrentState().GetSubStateArray()[1];
        }
        return null;
    }

    private BasicState ShouldTransistionToWallRunWallJumpState()
    {
        if (jumpPressed)
        {
            return playerMachine.GetCurrentState().GetSubStateArray()[2];
        }
        return null;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}