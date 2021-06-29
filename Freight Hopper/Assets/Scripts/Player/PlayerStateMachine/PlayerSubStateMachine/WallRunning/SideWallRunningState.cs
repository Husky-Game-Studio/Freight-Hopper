using System.Collections.Generic;
using System;

public class SideWallRunningState : PlayerState
{
    public SideWallRunningState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override BasicState TransitionState()
    {
        return CheckTransitions();
    }

    public override void PerformBehavior()
    {
        bool[] status = playerMachineCenter.abilities.wallRunBehavior.WallStatus();
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