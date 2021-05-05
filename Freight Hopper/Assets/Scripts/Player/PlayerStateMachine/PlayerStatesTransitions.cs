using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Help developing this and related code using this YouTube video: https://www.youtube.com/watch?v=V75hgcsCGOM&t=944s
// Delagates (func), actions, or function pointers to store the functions.
/* Example
    private class Transition
    {
      public Func<bool> Condition {get; }
      public IState To { get; }

      public Transition(IState to, Func<bool> condition)
      {
         To = to;
         Condition = condition;
      }
    }
*/


public class PlayerStatesTransitions
{
    private PlayerMachineCenter playerMachine;

    // Input Trackers //////////////////////////////////////////////////////////////////////////////////////////////////////////
    private bool jumpPressed = false;
    private bool grapplePressed = false;
    private bool groundPoundPressed = false;
    private bool upwardDashPressed = false;
    private bool fullStopPressed = false;
    private bool burstPressed = false;
    private bool releasedJumpPressed = false;

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
    private void ReleasedJumpButtonPressed()
    {
        releasedJumpPressed = true;
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public PlayerStatesTransitions(FiniteStateMachineCenter machineCenter) {
        playerMachine = (PlayerMachineCenter)machineCenter;
    }






    /* Common Transitions for the small prototype
    
    grapple pole -- although what happens in the if check is slightly different for the jumptransition to the grapple pole
    
    fall -- has different if checks but performs the same logic
    
    
    */

    /*
    // Common transition check to grapple pole
    public bool shouldTransitionToGrapplePoleState(BasicState currentState) {
        bool commonCheck = grapplePressed && !playerMachine.abilities.grapplePoleBehavior.Consumed &&
                           playerMachine.abilities.grapplePoleBehavior.Unlocked;
        if ((commonCheck) && (currentState != playerMachine.jumpState))
        {
            playerMachine.abilities.grapplePoleBehavior.PreventConsumption();
            return true;
        } else if ((commonCheck) && (currentState == playerMachine.jumpState)) {
            return true;
        }
        return false;
    }
    */

    // Common transistion check to fall
    public BasicState shouldTransitionToFallState() {
        if (((releasedJumpPressed || !playerMachine.jumpHoldingTimer.TimerActive()) && (playerMachine.GetCurrentState() == playerMachine.jumpState)) ||
            ((!playerMachine.playerCM.IsGrounded.current) && (playerMachine.GetCurrentState() != playerMachine.jumpState))) {
            return playerMachine.fallState;
        }
        return null;
    }

    // Common transition check to jump
    private BasicState shouldTransitionToJump() {
        if ((playerMachine.GetCurrentState() != playerMachine.fallState && (jumpPressed || playerMachine.jumpBufferTimer.TimerActive())
                    && (!playerMachine.abilities.jumpBehavior.Consumed && playerMachine.abilities.jumpBehavior.Unlocked)) ||
            (playerMachine.GetCurrentState() == playerMachine.fallState && (jumpPressed && playerMachine.coyoteeTimer.TimerActive()
                    && playerMachine.GetPreviousState() != playerMachine.jumpState && !playerMachine.abilities.jumpBehavior.Consumed
                    && playerMachine.abilities.jumpBehavior.Unlocked))
        ) {
            return playerMachine.jumpState;
        }
        return null;
    }

    // Common transition check to idle
    private BasicState shouldTransisitonToIdle() {
        if ((playerMachine.playerCM.IsGrounded.current && playerMachine.GetCurrentState() == playerMachine.fallState) ||
            (UserInput.Instance.Move() == Vector3.zero && playerMachine.GetCurrentState() == playerMachine.runState)) {
            return playerMachine.idleState;
        }
        return null;
    }

    // Common transition check to run
    private BasicState shouldTransitionToRun() {
        if (UserInput.Instance.Move() != Vector3.zero && playerMachine.abilities.movementBehavior.Unlocked) {
            return playerMachine.runState;
        }
        return null;
    }
    
    /*
    public BasicState toIdleState => shouldTransisitonToIdle();
    public BasicState toRunState => shouldTransitionToRun();
    public BasicState toJumpState => shouldTransitionToJump();
    public BasicState toFallState => shouldTransitionToFallState(); 
    */


    // Jump State Transitions ////////////////////////////////////////////////////////////////////////////////////////////////
    public BasicState JumpTransitions() {
        // Fall
        if (releasedJumpPressed || !playerMachine.jumpHoldingTimer.TimerActive())
        {
            return playerMachine.fallState;
        }
        // Grapple pole
        if (grapplePressed && !playerMachine.abilities.grapplePoleBehavior.Consumed && playerMachine.abilities.grapplePoleBehavior.Unlocked)
        {
            return playerMachine.grapplePoleState;
        }

        // Jump
        return playerMachine.jumpState;
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Idle State Transitions ////////////////////////////////////////////////////////////////////////////////////////////////
    public BasicState IdleTransitions() {
        // Jump
        if ((jumpPressed || playerMachine.jumpBufferTimer.TimerActive()) && !playerMachine.abilities.jumpBehavior.Consumed && playerMachine.abilities.jumpBehavior.Unlocked)
        {
            return playerMachine.jumpState;
        }

        // Grapple pole
        if (grapplePressed && !playerMachine.abilities.grapplePoleBehavior.Consumed && playerMachine.abilities.grapplePoleBehavior.Unlocked)
        {
            playerMachine.abilities.grapplePoleBehavior.PreventConsumption();
            return playerMachine.grapplePoleState;
        }

        // Ground Pound
        if (groundPoundPressed &&
            (playerMachine.playerCM.ContactNormal.current != playerMachine.playerCM.ValidUpAxis ||
            playerMachine.playerCM.IsGrounded.current == false) && !playerMachine.abilities.groundPoundBehavior.Consumed
            && playerMachine.abilities.groundPoundBehavior.Unlocked)
        {
            playerMachine.abilities.groundPoundBehavior.PreventConsumption();
            return playerMachine.groundPoundState;
        }

        // Upward Dash
        if (upwardDashPressed && !playerMachine.abilities.upwardDashBehavior.Consumed && playerMachine.abilities.upwardDashBehavior.Unlocked)
        {
            playerMachine.abilities.upwardDashBehavior.PreventConsumption();
            return playerMachine.upwardDashState;
        }

        // Full Stop
        if (fullStopPressed && !playerMachine.abilities.fullstopBehavior.Consumed && playerMachine.abilities.fullstopBehavior.Unlocked)
        {
            return playerMachine.fullStopState;
        }

        // Burst
        if (burstPressed && !playerMachine.abilities.burstBehavior.Consumed && playerMachine.abilities.burstBehavior.Unlocked)
        {
            playerMachine.abilities.burstBehavior.PreventConsumption();
            return playerMachine.burstState;
        }

        // Fall
        if (!playerMachine.playerCM.IsGrounded.current)
        {
            return playerMachine.fallState;
        }
        // Run
        if (UserInput.Instance.Move() != Vector3.zero && playerMachine.abilities.movementBehavior.Unlocked)
        {
            return playerMachine.runState;
        }

        jumpPressed = false;
        groundPoundPressed = false;
        grapplePressed = false;
        upwardDashPressed = false;
        fullStopPressed = false;
        burstPressed = false;
        return playerMachine.idleState;
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    // Run State Transitions /////////////////////////////////////////////////////////////////////////////////////////////////
    public BasicState RunTransitions() {
        // Jump
        if ((jumpPressed || playerMachine.jumpBufferTimer.TimerActive()) && !playerMachine.abilities.jumpBehavior.Consumed && playerMachine.abilities.jumpBehavior.Unlocked)
        {
            return playerMachine.jumpState;
        }

        // Grapple pole
        if (grapplePressed && !playerMachine.abilities.grapplePoleBehavior.Consumed && playerMachine.abilities.grapplePoleBehavior.Unlocked)
        {
            playerMachine.abilities.grapplePoleBehavior.PreventConsumption();
            return playerMachine.grapplePoleState;
        }

        // Ground Pound
        if (groundPoundPressed &&
            (playerMachine.playerCM.ContactNormal.current != playerMachine.playerCM.ValidUpAxis ||
            playerMachine.playerCM.IsGrounded.current == false) && !playerMachine.abilities.groundPoundBehavior.Consumed
            && playerMachine.abilities.groundPoundBehavior.Unlocked)
        {
            playerMachine.abilities.groundPoundBehavior.PreventConsumption();
            return playerMachine.groundPoundState;
        }

        // Upward Dash
        if (upwardDashPressed && !playerMachine.abilities.upwardDashBehavior.Consumed && playerMachine.abilities.upwardDashBehavior.Unlocked)
        {
            playerMachine.abilities.upwardDashBehavior.PreventConsumption();
            return playerMachine.upwardDashState;
        }

        // Full Stop
        if (fullStopPressed && !playerMachine.abilities.fullstopBehavior.Consumed && playerMachine.abilities.fullstopBehavior.Unlocked)
        {
            return playerMachine.fullStopState;
        }

        // Burst
        if (burstPressed && !playerMachine.abilities.burstBehavior.Consumed && playerMachine.abilities.burstBehavior.Unlocked)
        {
            playerMachine.abilities.burstBehavior.PreventConsumption();
            return playerMachine.burstState;
        }

        // Fall
        if (!playerMachine.playerCM.IsGrounded.current)
        {
            return playerMachine.fallState;
        }
        // Idle
        if (UserInput.Instance.Move() == Vector3.zero)
        {
            return playerMachine.idleState;
        }

        jumpPressed = false;
        groundPoundPressed = false;
        grapplePressed = false;
        upwardDashPressed = false;
        fullStopPressed = false;
        burstPressed = false;
        return playerMachine.runState;
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    // Fall State Transitions ////////////////////////////////////////////////////////////////////////////////////////////////
    

    public BasicState FallTransitions() {
    // Jump
        if (jumpPressed && playerMachine.coyoteeTimer.TimerActive() && playerMachine.GetPreviousState() != playerMachine.jumpState &&
            !playerMachine.abilities.jumpBehavior.Consumed && playerMachine.abilities.jumpBehavior.Unlocked)
        {
            return playerMachine.jumpState;
        }
        // Double Jump
        if (jumpPressed && !playerMachine.abilities.doubleJumpBehavior.Consumed && playerMachine.abilities.doubleJumpBehavior.Unlocked)
        {
            return playerMachine.doubleJumpState;
        }
        // Ground Pound
        if (groundPoundPressed &&
            (playerMachine.playerCM.ContactNormal.current != playerMachine.playerCM.ValidUpAxis ||
            playerMachine.playerCM.IsGrounded.current == false) && !playerMachine.abilities.groundPoundBehavior.Consumed
            && playerMachine.abilities.groundPoundBehavior.Unlocked)
        {
            return playerMachine.groundPoundState;
        }
        // Grapple pole
        if (grapplePressed && !playerMachine.abilities.grapplePoleBehavior.Consumed && playerMachine.abilities.grapplePoleBehavior.Unlocked)
        {
            return playerMachine.grapplePoleState;
        }

        // Upward Dash
        if (upwardDashPressed && !playerMachine.abilities.upwardDashBehavior.Consumed && playerMachine.abilities.upwardDashBehavior.Unlocked)
        {
            return playerMachine.upwardDashState;
        }

        // Full Stop
        if (fullStopPressed && !playerMachine.abilities.fullstopBehavior.Consumed && playerMachine.abilities.fullstopBehavior.Unlocked)
        {
            return playerMachine.fullStopState;
        }

        // Burst
        if (burstPressed && !playerMachine.abilities.burstBehavior.Consumed && playerMachine.abilities.burstBehavior.Unlocked)
        {
            return playerMachine.burstState;
        }

        // Idle
        if (playerMachine.playerCM.IsGrounded.current)
        {
            return playerMachine.idleState;
        }

        // Wall run
        if (playerMachine.abilities.wallRunBehavior.Unlocked && playerMachine.abilities.wallRunBehavior.Unlocked)
        {
            bool[] walls = playerMachine.abilities.wallRunBehavior.CheckWalls();
            if (walls[0] || walls[1] || walls[3])
            {
                return playerMachine.wallRunState;
            }
        }

        jumpPressed = false;
        groundPoundPressed = false;
        grapplePressed = false;
        upwardDashPressed = false;
        fullStopPressed = false;
        burstPressed = false;
        return playerMachine.fallState;
    }

    
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class toRunTransition : StateTransition {
        public bool shouldTransition(FiniteStateMachineCenter machineCenter) {
            PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
            return UserInput.Instance.Move() != Vector3.zero && playerMachine.abilities.movementBehavior.Unlocked;
        }
    }

    public class toIdleTransition : StateTransition {
        public bool shouldTransition(FiniteStateMachineCenter machineCenter) {
            PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
            return UserInput.Instance.Move() == Vector3.zero;
        }
    }

    public class toJumpTransition : StateTransition {
        public bool shouldTransition(FiniteStateMachineCenter machineCenter) {
            PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
            return false;//(jumpPressed || playerMachine.jumpBufferTimer.TimerActive()) 
                    //&& !playerMachine.abilities.jumpBehavior.Consumed && playerMachine.abilities.jumpBehavior.Unlocked);
        }
    }

    public class toFallTransition : StateTransition {
        public bool shouldTransition(FiniteStateMachineCenter machineCenter) {
            PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
            return UserInput.Instance.Move() != Vector3.zero && playerMachine.abilities.movementBehavior.Unlocked;
        }
    }
}