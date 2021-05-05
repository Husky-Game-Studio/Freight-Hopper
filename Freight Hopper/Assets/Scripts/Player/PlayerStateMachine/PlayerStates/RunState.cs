using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : BasicState
{
    private bool jumpPressed = false;
    private bool grapplePressed = false;
    private bool groundPoundPressed = false;
    private bool upwardDashPressed = false;
    private bool fullStopPressed = false;
    private bool burstPressed = false;


    public RunState(List<BasicState> myTransitions) {
        this.stateTransitions = myTransitions;
    }

    public override void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        UserInput.Instance.JumpInput += this.JumpButtonPressed;
        UserInput.Instance.GrappleInput += this.GrappleButtonPressed;
        UserInput.Instance.GroundPoundInput += this.GroundPoundButtonPressed;
        UserInput.Instance.UpwardDashInput += this.UpwardDashPressed;
        UserInput.Instance.FullStopInput += this.FullStopPressed;
        UserInput.Instance.BurstInput += this.BurstPressed;

        playerMachine.coyoteeTimer.DeactivateTimer();
    }

    public override void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        UserInput.Instance.JumpInput -= this.JumpButtonPressed;
        UserInput.Instance.GrappleInput -= this.GrappleButtonPressed;
        UserInput.Instance.GroundPoundInput -= this.GroundPoundButtonPressed;
        UserInput.Instance.UpwardDashInput -= this.UpwardDashPressed;
        UserInput.Instance.FullStopInput -= this.FullStopPressed;
        UserInput.Instance.BurstInput -= this.BurstPressed;

        jumpPressed = false;
        groundPoundPressed = false;
        grapplePressed = false;
        upwardDashPressed = false;
        fullStopPressed = false;
        burstPressed = false;
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
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
        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
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
}