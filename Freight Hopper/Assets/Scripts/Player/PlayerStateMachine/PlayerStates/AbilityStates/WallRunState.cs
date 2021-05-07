using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallRunState : BasicState
{
    private PlayerSubStateMachineCenter pSSMC;
    private PlayerMachineCenter myPlayerMachineCenter;
    private BasicState[] miniStateArray;

    public WallRunState(PlayerMachineCenter myPMC, List<Func<BasicState>> myTransitions)
    {
        this.stateTransitions = myTransitions;
        myPlayerMachineCenter = myPMC;
        miniStateArray = new BasicState[3];
        miniStateArray[0] = null;//new SideWallRunningState();
        miniStateArray[1] = new WallClimbingState();
        miniStateArray[2] = new WallJumpState();

        pSSMC = new PlayerSubStateMachineCenter(this, miniStateArray, myPlayerMachineCenter);
    }

    public override void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter = playerMachine;
        pSSMC.SetPrevCurrState(miniStateArray[0]);
        pSSMC.GetCurrentSubState().SubToListeners(playerMachine);
        playerMachine.abilities.wallRunBehavior.EntryAction();
        //UserInput.Instance.JumpInputCanceled += ReleaseJump;
    }

    public override void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        if (pSSMC.currentState == miniStateArray[1])
        {
            myPlayerMachineCenter.abilities.wallRunBehavior.WallClimbExit();
        }
        pSSMC.GetCurrentSubState().UnsubToListeners(myPlayerMachineCenter);
        myPlayerMachineCenter.abilities.wallRunBehavior.ExitAction();
        playerMachine.pFSMTH.releasedJumpPressed = false;
        //UserInput.Instance.JumpInputCanceled -= ReleaseJump;
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        foreach (Func<BasicState> stateCheck in this.stateTransitions) {
            BasicState tempState = stateCheck();
            if (tempState != null) {
                return tempState;
            }
        }

        // bool[] status = myPlayerMachineCenter.abilities.wallRunBehavior.CheckWalls();
        // // Fall from wall climb
        // if (!status[0] && !status[1] && !status[3] && pSSMC.currentState != miniStateArray[2] && pSSMC.currentState != miniStateArray[1])
        // {
        //     myPlayerMachineCenter.abilities.wallRunBehavior.coyoteTimer.CountDownFixed();
        //     if (!myPlayerMachineCenter.abilities.wallRunBehavior.coyoteTimer.TimerActive())
        //     {
        //         myPlayerMachineCenter.abilities.wallRunBehavior.coyoteTimer.ResetTimer();
        //         return myPlayerMachineCenter.fallState;
        //     }
        // }
        // if (myPlayerMachineCenter.playerCM.IsGrounded.current)
        // {
        //     return myPlayerMachineCenter.idleState;
        // }
        // if (pSSMC.currentState == miniStateArray[2] && (releasedJump || !myPlayerMachineCenter.abilities.wallRunBehavior.jumpHoldingTimer.TimerActive()))
        // {
        //     return myPlayerMachineCenter.fallState;
        // }
        // if (pSSMC.currentState == miniStateArray[1] && !myPlayerMachineCenter.abilities.wallRunBehavior.climbTimer.TimerActive())
        // {
        //     return myPlayerMachineCenter.fallState;
        // }
        playerMachine.pFSMTH.releasedJumpPressed = false;
        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        pSSMC.PerformSubMachineBehavior();
        myPlayerMachineCenter.abilities.wallRunBehavior.Action();
        myPlayerMachineCenter.abilities.movementBehavior.PlayerMove();
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

    public PlayerSubStateMachineCenter GetPlayerSubStateMachineCenter() {
        return pSSMC;
    }
}