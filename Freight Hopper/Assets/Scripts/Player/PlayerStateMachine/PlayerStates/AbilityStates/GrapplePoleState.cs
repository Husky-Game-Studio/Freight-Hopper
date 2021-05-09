using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GrapplePoleState : BasicState
{
    private PlayerSubStateMachineCenter pSSMC;
    private PlayerMachineCenter myPlayerMachineCenter;
    private BasicState[] miniStateArray;

    public GrapplePoleState(PlayerMachineCenter myPMC, List<Func<BasicState>> myTransitions)
    {
        this.stateTransitions = myTransitions;

        myPlayerMachineCenter = myPMC;
        miniStateArray = new BasicState[2];
        miniStateArray[0] = null;//new GrappleFireState();
        miniStateArray[1] = new GrappleAnchoredState();
        pSSMC = new PlayerSubStateMachineCenter(this, miniStateArray, myPlayerMachineCenter);
    }

    public override void EnterState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        //UserInput.Instance.GrappleInput += this.GrapplePolePressed;
        //UserInput.Instance.JumpInput += this.JumpButtonPressed;
        pSSMC.SetPrevCurrState(miniStateArray[0]);
        pSSMC.GetCurrentSubState().EnterState(playerMachine);
    }

    public override void ExitState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        //UserInput.Instance.GrappleInput -= this.GrapplePolePressed;
        //UserInput.Instance.JumpInput -= this.JumpButtonPressed;

        playerMachine.pFSMTH.grapplePressed = false;
        playerMachine.pFSMTH.jumpPressed = false;
        pSSMC.GetCurrentSubState().ExitState(playerMachine);
        playerMachine.abilities.grapplePoleBehavior.ExitAction();
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

        // if (grapplePolePressed ||
        //     (playerMachine.abilities.grapplePoleBehavior.GrapplePoleBroken() && playerMachine.abilities.grapplePoleBehavior.IsAnchored()))
        // {
        //     return playerMachine.fallState;
        // }

        // if (jumpPressed)
        // {
        //     return playerMachine.jumpState;
        // }
        playerMachine.pFSMTH.grapplePressed = false;
        playerMachine.pFSMTH.jumpPressed = false;
        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        pSSMC.PerformSubMachineBehavior();
    }

    public override bool HasSubStateMachine()
    {
        return true;
    }

    public override BasicState GetCurrentSubState()
    {
        return pSSMC.GetCurrentSubState();
    }

    public override BasicState[] GetSubStateArray()
    {
        return miniStateArray;
    }
}