using System.Collections.Generic;
using System;

public class SideWallRunningState : PlayerState
{
    public SideWallRunningState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void ExitState()
    {
        playerMachineCenter.pFSMTH.ResetInputs();
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

        playerMachineCenter.pFSMTH.ResetInputs();
        return this;
    }

    public override void PerformBehavior()
    {
        bool[] status = playerMachineCenter.abilities.wallRunBehavior.CheckWalls();
        if (status[1])
        {
            playerMachineCenter.abilities.wallRunBehavior.RightWallRun();
        }
        if (status[3])
        {
            playerMachineCenter.abilities.wallRunBehavior.LeftWallRun();
        }
    }
}