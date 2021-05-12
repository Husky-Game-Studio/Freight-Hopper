using System;
using System.Collections.Generic;

public class GrapplePoleAnchoredState : PlayerState
{
    public GrapplePoleAnchoredState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EntryState()
    {
        playerMachineCenter.abilities.grapplePoleBehavior.EntryAction();
    }

    public override void ExitState()
    {
        playerMachineCenter.abilities.grapplePoleBehavior.ExitAction();
    }

    public override BasicState TransitionState()
    {
        return CheckTransitions();
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.abilities.grapplePoleBehavior.Grapple(UserInput.Instance.Move());

        if (playerMachineCenter.playerCM.IsGrounded.current && !playerMachineCenter.playerCM.IsGrounded.old)
        {
            playerMachineCenter.abilities.Recharge();
            playerMachineCenter.abilities.grapplePoleBehavior.PreventConsumption();
        }
    }
}