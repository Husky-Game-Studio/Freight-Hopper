using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallJumpState : BasicState
{
    private PlayerMachineCenter myPlayerMachineCenter;

    public override void EnterState(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter.abilities.wallRunBehavior.jumpHoldingTimer.ResetTimer();
        myPlayerMachineCenter.abilities.wallRunBehavior.WallJumpInitial();
    }

    public override void ExitState(FiniteStateMachineCenter machineCenter)
    {
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter.abilities.wallRunBehavior.WallJumpContinous();
    }
}