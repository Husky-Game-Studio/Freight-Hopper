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
        IList<bool> status = playerMachineCenter.wallRunBehavior.WallStatus;
        if (status[2])
        {
            playerMachineCenter.wallRunBehavior.RightWallRun();
        }
        if (status[0])
        {
            playerMachineCenter.wallRunBehavior.LeftWallRun();
        }
    }
}