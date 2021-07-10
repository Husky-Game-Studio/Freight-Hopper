using System.Collections.Generic;
using System;

public class GrappleGroundPoundState : PlayerState
{
    public GrappleGroundPoundState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EntryState()
    {
        if (!playerMachineCenter.initialGroundPoundBurstCoolDown.TimerActive())
        {
            playerMachineCenter.abilities.groundPoundBehavior.EntryAction();
            playerMachineCenter.initialGroundPoundBurstCoolDown.ResetTimer();
        }
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

        if (playerMachineCenter.collisionManagement.IsGrounded.current && !playerMachineCenter.collisionManagement.IsGrounded.old)
        {
            playerMachineCenter.abilities.Recharge();
            playerMachineCenter.abilities.groundPoundBehavior.PreventConsumption();
            playerMachineCenter.abilities.grapplePoleBehavior.PreventConsumption();
        }
    }
}