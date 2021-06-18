using System.Collections.Generic;
using System;

public class WallRunState : PlayerState
{
    private PlayerSubStateMachineCenter pSSMC;
    private BasicState[] miniStateArray;

    private WallRunSubStateMachineCenter wRSSMC;

    public WallRunState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
        miniStateArray = new BasicState[3];
        miniStateArray[0] = null; // Done already by playerMachineCenter
        miniStateArray[1] = new WallClimbingState(playerMachineCenter, myTransitions);
        miniStateArray[2] = new WallJumpState(playerMachineCenter, myTransitions);

        pSSMC = new PlayerSubStateMachineCenter(this, miniStateArray, playerMachineCenter);

        wRSSMC = new WallRunSubStateMachineCenter();
    }

    public override void EntryState()
    {
        pSSMC.SetPrevCurrState(miniStateArray[0]);
        pSSMC.GetCurrentSubState().EntryState();
        playerMachineCenter.abilities.wallRunBehavior.EntryAction();
    }

    public override void ExitState()
    {
        if (pSSMC.currentState == miniStateArray[1])
        {
            playerMachineCenter.abilities.wallRunBehavior.WallClimbExit();
        }

        pSSMC.GetCurrentSubState().ExitState();
        playerMachineCenter.abilities.wallRunBehavior.ExitAction();
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();

        return state;
    }

    public override void PerformBehavior()
    {
        pSSMC.PerformSubMachineBehavior();
        playerMachineCenter.abilities.wallRunBehavior.Action();
        playerMachineCenter.abilities.movementBehavior.PlayerMove();
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

    public PlayerSubStateMachineCenter GetPlayerSubStateMachineCenter()
    {
        return pSSMC;
    }
}