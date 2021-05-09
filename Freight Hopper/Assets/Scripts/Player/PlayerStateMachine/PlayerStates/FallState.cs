using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FallState : BasicState
{
    public FallState(List<Func<BasicState>> myTransitions)
    {
        this.stateTransitions = myTransitions;
    }

    public override void EnterState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        // UserInput.Instance.JumpInput += this.JumpButtonPressed;
        // UserInput.Instance.GrappleInput += this.GrappleButtonPressed;
        // UserInput.Instance.GroundPoundInput += this.GroundPoundButtonPressed;
        // UserInput.Instance.UpwardDashInput += this.UpwardDashPressed;
        // UserInput.Instance.FullStopInput += this.FullStopPressed;
        // UserInput.Instance.BurstInput += this.BurstPressed;

        if (playerMachine.GetPreviousState() != playerMachine.jumpState)
        {
            playerMachine.coyoteeTimer.ResetTimer();
        }
        else
        {
            playerMachine.coyoteeTimer.DeactivateTimer();
        }
    }

    public override void ExitState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        playerMachine.coyoteeTimer.DeactivateTimer();
        playerMachine.pFSMTH.jumpPressed = false;
        playerMachine.pFSMTH.groundPoundPressed = false;
        playerMachine.pFSMTH.grapplePressed = false;
        playerMachine.pFSMTH.upwardDashPressed = false;
        playerMachine.pFSMTH.releasedUpwardDash = false;
        playerMachine.pFSMTH.fullStopPressed = false;
        playerMachine.pFSMTH.burstPressed = false;
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        foreach (Func<BasicState> stateCheck in this.stateTransitions)
        {
            BasicState tempState = stateCheck();
            if (tempState != null)
            {
                return tempState;
            }
        }

        // if coyetee timer is not expired and the jump button was pressed-> then jump
        // Jump
        // if (jumpPressed && playerMachine.coyoteeTimer.TimerActive() && playerMachine.GetPreviousState() != playerMachine.jumpState &&
        //     !playerMachine.abilities.jumpBehavior.Consumed && playerMachine.abilities.jumpBehavior.Unlocked)
        // {
        //     return playerMachine.jumpState;
        // }
        // Double Jump
        // if (jumpPressed && !playerMachine.abilities.doubleJumpBehavior.Consumed && playerMachine.abilities.doubleJumpBehavior.Unlocked)
        // {
        //     return playerMachine.doubleJumpState;
        // }
        // Ground Pound
        // if (groundPoundPressed &&
        //     (playerMachine.playerCM.ContactNormal.current != playerMachine.playerCM.ValidUpAxis ||
        //     playerMachine.playerCM.IsGrounded.current == false) && !playerMachine.abilities.groundPoundBehavior.Consumed
        //     && playerMachine.abilities.groundPoundBehavior.Unlocked)
        // {
        //     return playerMachine.groundPoundState;
        // }
        // Grapple pole
        // if (grapplePressed && !playerMachine.abilities.grapplePoleBehavior.Consumed && playerMachine.abilities.grapplePoleBehavior.Unlocked)
        // {
        //     return playerMachine.grapplePoleState;
        // }

        // Upward Dash
        // if (upwardDashPressed && !playerMachine.abilities.upwardDashBehavior.Consumed && playerMachine.abilities.upwardDashBehavior.Unlocked)
        // {
        //     return playerMachine.upwardDashState;
        // }

        // Full Stop
        // if (fullStopPressed && !playerMachine.abilities.fullstopBehavior.Consumed && playerMachine.abilities.fullstopBehavior.Unlocked)
        // {
        //     return playerMachine.fullStopState;
        // }

        // Burst
        // if (burstPressed && !playerMachine.abilities.burstBehavior.Consumed && playerMachine.abilities.burstBehavior.Unlocked)
        // {
        //     return playerMachine.burstState;
        // }

        // // Idle
        // if (playerMachine.playerCM.IsGrounded.current)
        // {
        //     return playerMachine.idleState;
        // }

        // Wall run
        // if (playerMachine.abilities.wallRunBehavior.Unlocked && playerMachine.abilities.wallRunBehavior.Unlocked)
        // {
        //     bool[] walls = playerMachine.abilities.wallRunBehavior.CheckWalls();
        //     if (walls[0] || walls[1] || walls[3])
        //     {
        //         return playerMachine.wallRunState;
        //     }
        // }

        playerMachine.pFSMTH.jumpPressed = false;

        playerMachine.pFSMTH.releasedJumpPressed = false;

        playerMachine.pFSMTH.groundPoundPressed = false;
        playerMachine.pFSMTH.grapplePressed = false;
        playerMachine.pFSMTH.upwardDashPressed = false;
        playerMachine.pFSMTH.fullStopPressed = false;
        playerMachine.pFSMTH.burstPressed = false;
        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        if (playerMachine.GetPreviousState() != playerMachine.jumpState)
        {
            playerMachine.coyoteeTimer.CountDown();
        }

        playerMachine.abilities.movementBehavior.PlayerMove();
    }
}