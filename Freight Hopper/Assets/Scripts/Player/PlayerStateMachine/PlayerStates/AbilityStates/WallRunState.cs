using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunState : BasicState
{
    private PlayerSubStateMachineCenter pSSMC;
    private PlayerMachineCenter myPlayerMachineCenter;
    private BasicState[] miniStateArray;

    public WallRunState(PlayerMachineCenter myPMC)
    {
        myPlayerMachineCenter = myPMC;
        miniStateArray = new BasicState[3];
        miniStateArray[0] = new SideWallRunningState();
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
    }

    public override void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        if (pSSMC.currentState == miniStateArray[1])
        {
            myPlayerMachineCenter.abilities.wallRunBehavior.WallClimbExit();
        }
        pSSMC.GetCurrentSubState().UnsubToListeners(myPlayerMachineCenter);
        myPlayerMachineCenter.abilities.wallRunBehavior.ExitAction();
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        bool[] status = myPlayerMachineCenter.abilities.wallRunBehavior.CheckWalls();
        // Fall from wall climb
        if (!status[0] && !status[1] && !status[3])
        {
            Debug.Log("Not touching any walls leaving");
            return myPlayerMachineCenter.fallState;
        }
        if (pSSMC.currentState == miniStateArray[1] || pSSMC.currentState == miniStateArray[2])
        {
            return myPlayerMachineCenter.fallState;
        }
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
}