using System.Collections.Generic;
using System;

public class GrappleBurst : PlayerState
{
    public GrappleBurst(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void ExitState()
    {
        playerMachineCenter.abilities.burstBehavior.ExitAction();
    }

    public override BasicState TransitionState()
    {
        return playerMachineCenter.grapplePoleState.GetSubStateArray()[0];
    }

    public override void PerformBehavior()
    {
        if (playerMachineCenter.playerCM.IsGrounded.current)
        {
            playerMachineCenter.abilities.burstBehavior.GroundBurst();
        }
        else
        {
            if (playerMachineCenter.abilities.wallRunBehavior.CheckWalls()[2])
            {
                playerMachineCenter.abilities.burstBehavior.WallBurst();
            }
            else
            {
                playerMachineCenter.abilities.burstBehavior.AirBurst();
            }
        }
    }
}