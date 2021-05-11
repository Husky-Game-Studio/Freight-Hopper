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
    public class Toggle
    {
        public bool value = false;

        public void Reset() => value = false;

        public void Trigger() => value = true;
    }

    public Toggle jumpPressed = new Toggle();
    public Toggle releasedJump = new Toggle();
    public Toggle grapplePressed = new Toggle();
    public Toggle groundPoundPressed = new Toggle();
    public Toggle groundPoundReleased = new Toggle();
    public Toggle upwardDashPressed = new Toggle();
    public Toggle fullStopPressed = new Toggle();
    public Toggle burstPressed = new Toggle();
    public Toggle releasedUpwardDash = new Toggle();

    public void ResetInputs()
    {
        jumpPressed.Reset();
        releasedJump.Reset();
        grapplePressed.Reset();
        groundPoundPressed.Reset();
        groundPoundReleased.Reset();
        upwardDashPressed.Reset();
        fullStopPressed.Reset();
        burstPressed.Reset();
        releasedUpwardDash.Reset();
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
    public Func<BasicState> checkToGrappleGroundPoundState;
    public Func<BasicState> checkToWallRunState;
    public Func<BasicState> checkToWallRunWallClimbingState;
    public Func<BasicState> checkToWallRunWallJumpState;

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnDisable()
    {
        this.UnsubToListeners();
    }

    private void SubToListeners()
    {
        UserInput.Instance.JumpInput += jumpPressed.Trigger;
        UserInput.Instance.JumpInputCanceled += releasedJump.Trigger;
        UserInput.Instance.GrappleInput += grapplePressed.Trigger;
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
        UserInput.Instance.JumpInputCanceled -= releasedJump.Trigger;
        UserInput.Instance.GrappleInput -= grapplePressed.Trigger;
        UserInput.Instance.GroundPoundInput -= groundPoundPressed.Trigger;
        UserInput.Instance.GroundPoundCanceled -= groundPoundReleased.Trigger;
        UserInput.Instance.UpwardDashInput -= upwardDashPressed.Trigger;
        UserInput.Instance.FullStopInput -= fullStopPressed.Trigger;
        UserInput.Instance.BurstInput -= burstPressed.Trigger;
        UserInput.Instance.UpwardDashInputCanceled -= releasedUpwardDash.Trigger;
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
        checkToGrappleGroundPoundState = ShouldTransitionToGrappleGroundPoundState;
        checkToWallRunState = ShouldTransitionToWallRunState;
        checkToWallRunWallClimbingState = ShouldTransistionToWallRunWallClimbingState;
        checkToWallRunWallJumpState = ShouldTransistionToWallRunWallJumpState;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // The conditional functions to check if the PFSM should transition to a different state ///////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Common transistion check to fall
    private BasicState ShouldTransitionToFallState()
    {
        if ((releasedJump.value || !playerMachine.abilities.jumpBehavior.jumpHoldingTimer.TimerActive()) &&
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

        if (releasedUpwardDash.value && playerMachine.currentState == playerMachine.upwardDashState)
        {
            return playerMachine.fallState;
        }

        if (groundPoundReleased.value && playerMachine.currentState == playerMachine.groundPoundState)
        {
            return playerMachine.fallState;
        }

        if ((grapplePressed.value || (playerMachine.abilities.grapplePoleBehavior.GrapplePoleBroken() &&
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
                && (releasedJump.value || !playerMachine.abilities.wallRunBehavior.jumpHoldingTimer.TimerActive()))
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
        if ((playerMachine.GetCurrentState() != playerMachine.fallState && (jumpPressed.value || playerMachine.abilities.jumpBehavior.jumpBufferTimer.TimerActive())
                    && (!playerMachine.abilities.jumpBehavior.Consumed && playerMachine.abilities.jumpBehavior.Unlocked)) ||
            (playerMachine.GetCurrentState() == playerMachine.fallState && (jumpPressed.value && playerMachine.abilities.jumpBehavior.coyoteeTimer.TimerActive()
                    && playerMachine.GetPreviousState() != playerMachine.jumpState && playerMachine.abilities.jumpBehavior.UnlockedAndReady))
        )
        {
            return playerMachine.jumpState;
        }
        if ((jumpPressed.value && playerMachine.abilities.jumpBehavior.UnlockedAndReady) &&
            (playerMachine.currentState == playerMachine.groundPoundState))
        {
            return playerMachine.jumpState;
        }
        if (jumpPressed.value && playerMachine.currentState == playerMachine.grapplePoleState)
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
        if (jumpPressed.value && playerMachine.abilities.doubleJumpBehavior.UnlockedAndReady &&
            playerMachine.currentState == playerMachine.fallState)
        {
            return playerMachine.doubleJumpState;
        }
        if (jumpPressed.value && playerMachine.abilities.jumpBehavior.Consumed &&
            playerMachine.abilities.doubleJumpBehavior.UnlockedAndReady &&
            playerMachine.currentState == playerMachine.doubleJumpState)
        {
            return playerMachine.doubleJumpState;
        }
        return null;
    }

    private BasicState ShouldTransitionToGroundPoundState()
    {
        if (groundPoundPressed.value &&
            (playerMachine.playerCM.ContactNormal.current != playerMachine.playerCM.ValidUpAxis ||
            playerMachine.playerCM.IsGrounded.current == false) && playerMachine.abilities.groundPoundBehavior.UnlockedAndReady)
        {
            if (playerMachine.playerCM.IsGrounded.current)
            {
                playerMachine.abilities.groundPoundBehavior.PreventConsumption();
            }

            return playerMachine.groundPoundState;
        }

        return null;
    }

    private BasicState ShouldTransitionToGrappleGroundPoundState()
    {
        if (groundPoundPressed.value && playerMachine.currentState == playerMachine.grapplePoleState)
        {
            if ((playerMachine.playerCM.ContactNormal.current != playerMachine.playerCM.ValidUpAxis ||
                playerMachine.playerCM.IsGrounded.current == false) && playerMachine.abilities.groundPoundBehavior.UnlockedAndReady)
            {
                if (playerMachine.playerCM.IsGrounded.current)
                {
                    playerMachine.abilities.groundPoundBehavior.PreventConsumption();
                }

                return playerMachine.grapplePoleState.GetSubStateArray()[1];
            }
        }
        if (grapplePressed.value && playerMachine.currentState == playerMachine.groundPoundState)
        {
            if ((playerMachine.playerCM.ContactNormal.current != playerMachine.playerCM.ValidUpAxis ||
                playerMachine.playerCM.IsGrounded.current == false) && playerMachine.abilities.groundPoundBehavior.UnlockedAndReady)
            {
                if (playerMachine.playerCM.IsGrounded.current)
                {
                    playerMachine.abilities.groundPoundBehavior.PreventConsumption();
                }
                GrapplePoleState state = playerMachine.grapplePoleState;
                state.TransitioningFromGroundPound();
                return playerMachine.grapplePoleState;
            }
        }
        return null;
    }

    private BasicState ShouldTransitionToUpwardDashState()
    {
        if (upwardDashPressed.value && playerMachine.abilities.upwardDashBehavior.UnlockedAndReady)
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
        if (fullStopPressed.value && playerMachine.abilities.fullstopBehavior.UnlockedAndReady)
        {
            return playerMachine.fullStopState;
        }
        return null;
    }

    private BasicState ShouldTransitionToBurstState()
    {
        if (burstPressed.value && playerMachine.abilities.burstBehavior.UnlockedAndReady)
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
        if (playerMachine.abilities.grapplePoleBehavior.IsAnchored())
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
        if (jumpPressed.value)
        {
            return playerMachine.GetCurrentState().GetSubStateArray()[2];
        }
        return null;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}