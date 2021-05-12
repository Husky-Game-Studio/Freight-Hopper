using System.Collections.Generic;
using System;

public class GrappleGroundPoundState : PlayerState
{
    public GrappleGroundPoundState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EntryState()
    {
        playerMachineCenter.abilities.groundPoundBehavior.EntryAction();
    }

    public override void ExitState()
    {
        playerMachineCenter.abilities.groundPoundBehavior.ExitAction();
    }

    public override BasicState TransitionState()
    {
        return CheckTransitions();
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.abilities.grapplePoleBehavior.Grapple(UserInput.Instance.Move());
        playerMachineCenter.abilities.groundPoundBehavior.Action();

        if (playerMachineCenter.playerCM.IsGrounded.current && !playerMachineCenter.playerCM.IsGrounded.old)
        {
            playerMachineCenter.abilities.Recharge();
            playerMachineCenter.abilities.groundPoundBehavior.PreventConsumption();
            playerMachineCenter.abilities.grapplePoleBehavior.PreventConsumption();
        }
    }
}