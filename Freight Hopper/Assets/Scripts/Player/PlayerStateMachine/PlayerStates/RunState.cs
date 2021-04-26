using UnityEngine;

public class RunState : BasicState
{
    private bool jumpPressed = false;
    private bool grapplePressed = false;
    private bool groundPoundPressed = false;
    private bool upwardDashPressed = false;
    private bool fullStopPressed = false;
    private bool burstPressed = false;

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        UserInput.Input.JumpInput += this.JumpButtonPressed;
        UserInput.Input.GrappleInput += this.GrappleButtonPressed;
        UserInput.Input.GroundPoundInput += this.GroundPoundButtonPressed;
        UserInput.Input.UpwardDashInput += this.UpwardDashPressed;
        UserInput.Input.FullStopInput += this.FullStopPressed;
        UserInput.Input.BurstInput += this.BurstPressed;

        playerMachine.coyoteeTimer.DeactivateTimer();
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        UserInput.Input.JumpInput -= this.JumpButtonPressed;
        UserInput.Input.GrappleInput -= this.GrappleButtonPressed;
        UserInput.Input.GroundPoundInput -= this.GroundPoundButtonPressed;
        UserInput.Input.UpwardDashInput -= this.UpwardDashPressed;
        UserInput.Input.FullStopInput -= this.FullStopPressed;
        UserInput.Input.BurstInput -= this.BurstPressed;

        jumpPressed = false;
        groundPoundPressed = false;
        grapplePressed = false;
        upwardDashPressed = false;
        fullStopPressed = false;
        burstPressed = false;
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        // Jump
        if ((jumpPressed || playerMachine.jumpBufferTimer.TimerActive()) && !playerMachine.abilities.jumpBehavior.IsConsumed)
        {
            return playerMachine.jumpState;
        }

        // Double Jump
        if (jumpPressed && playerMachine.abilities.jumpBehavior.IsConsumed && !playerMachine.abilities.doubleJumpBehavior.IsConsumed)
        {
            return playerMachine.doubleJumpState;
        }

        // Grapple pole
        if (grapplePressed && !playerMachine.abilities.grapplePoleBehavior.IsConsumed)
        {
            playerMachine.abilities.grapplePoleBehavior.PreventConsumption();
            return playerMachine.grapplePoleState;
        }

        // Ground Pound
        if (groundPoundPressed &&
            (playerMachine.playerCM.ContactNormal.current != playerMachine.playerCM.ValidUpAxis ||
            playerMachine.playerCM.IsGrounded.current == false) && !playerMachine.abilities.groundPoundBehavior.IsConsumed)
        {
            return playerMachine.groundPoundState;
        }

        // Upward Dash
        if (upwardDashPressed && !playerMachine.abilities.upwardDashBehavior.IsConsumed)
        {
            return playerMachine.upwardDashState;
        }

        // Full Stop
        if (fullStopPressed && !playerMachine.abilities.fullstopBehavior.IsConsumed)
        {
            return playerMachine.fullStopState;
        }

        // Burst
        if (burstPressed && !playerMachine.abilities.burstBehavior.IsConsumed)
        {
            return playerMachine.burstState;
        }

        // Fall
        if (!playerMachine.playerCM.IsGrounded.current)
        {
            return playerMachine.fallState;
        }
        // Idle
        if (UserInput.Input.Move() == Vector3.zero)
        {
            return playerMachine.idleState;
        }

        jumpPressed = false;
        groundPoundPressed = false;
        grapplePressed = false;
        upwardDashPressed = false;
        fullStopPressed = false;
        burstPressed = false;
        return this;
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        playerMachine.abilities.movementBehavior.Action();
    }

    private void JumpButtonPressed()
    {
        jumpPressed = true;
    }

    private void GroundPoundButtonPressed()
    {
        groundPoundPressed = true;
    }

    private void GrappleButtonPressed()
    {
        grapplePressed = true;
    }

    private void UpwardDashPressed()
    {
        upwardDashPressed = true;
    }

    private void FullStopPressed()
    {
        fullStopPressed = true;
    }

    private void BurstPressed()
    {
        burstPressed = true;
    }

    public bool HasSubStateMachine()
    {
        return false;
    }

    public BasicState GetCurrentSubState()
    {
        return null;
    }

    public BasicState[] GetSubStateArray()
    {
        return null;
    }
}