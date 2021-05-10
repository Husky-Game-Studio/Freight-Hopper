using System.Collections.Generic;
using System;

public class WallRunState : PlayerState
{
    private PlayerSubStateMachineCenter pSSMC;
    private BasicState[] miniStateArray;

    public WallRunState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
        miniStateArray = new BasicState[3];
        miniStateArray[0] = null;//new SideWallRunningState();
        miniStateArray[1] = new WallClimbingState(playerMachineCenter, myTransitions);
        miniStateArray[2] = new WallJumpState(playerMachineCenter, myTransitions);

        pSSMC = new PlayerSubStateMachineCenter(this, miniStateArray, playerMachineCenter);
    }

    public override void EnterState()
    {
        pSSMC.SetPrevCurrState(miniStateArray[0]);
        pSSMC.GetCurrentSubState().EnterState();
        playerMachineCenter.abilities.wallRunBehavior.EntryAction();
    }

    public override void ExitState()
    {
        if (pSSMC.currentState == miniStateArray[1])
        {
            playerMachineCenter.abilities.wallRunBehavior.WallClimbExit();
        }
        pSSMC.GetCurrentSubState().ExitState();
        playerMachineCenter.abilities.wallRunBehavior.ExitAction();
        playerMachineCenter.pFSMTH.releasedJumpPressed = false;
    }

    public override BasicState TransitionState()
    {
        foreach (Func<BasicState> stateCheck in this.stateTransitions)
        {
            BasicState tempState = stateCheck();
            if (tempState != null)
            {
                return tempState;
            }
        }

        playerMachineCenter.pFSMTH.releasedJumpPressed = false;
        return this;
    }

    public override void PerformBehavior()
    {
        pSSMC.PerformSubMachineBehavior();
        playerMachineCenter.abilities.wallRunBehavior.Action();
        playerMachineCenter.abilities.movementBehavior.PlayerMove();
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

    public PlayerSubStateMachineCenter GetPlayerSubStateMachineCenter()
    {
        return pSSMC;
    }
}