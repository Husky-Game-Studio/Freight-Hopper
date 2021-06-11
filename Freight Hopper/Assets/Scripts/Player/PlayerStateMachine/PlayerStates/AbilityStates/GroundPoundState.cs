using System.Collections.Generic;
using System;

public class GroundPoundState : PlayerState
{
    public GroundPoundState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
    }

    public override void EntryState()
    {
        if (!playerMachineCenter.initialGroundPoundBurstCoolDown.TimerActive())
        {
            playerMachineCenter.abilities.groundPoundBehavior.EntryAction();
            playerMachineCenter.initialGroundPoundBurstCoolDown.ResetTimer();
        }
        playerMachineCenter.playerPM.friction.ReduceFriction(playerMachineCenter.abilities.groundPoundBehavior.FrictionReduction);
    }

    public override void ExitState()
    {
        playerMachineCenter.abilities.groundPoundBehavior.ExitAction();
        playerMachineCenter.playerPM.friction.ResetFrictionReduction();
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();

        return state;
    }

    public override void PerformBehavior()
    {
        playerMachineCenter.abilities.movementBehavior.PlayerMove();
        playerMachineCenter.abilities.groundPoundBehavior.Action();
    }
}