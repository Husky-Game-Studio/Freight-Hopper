using System.Collections.Generic;
using System;

public class GrapplePoleState : PlayerState
{
    private PlayerSubStateMachineCenter pSSMC;
    private BasicState[] miniStateArray;

    public GrapplePoleState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
        this.stateTransitions = myTransitions;

        miniStateArray = new BasicState[1];
        miniStateArray[0] = new GrappleAnchoredState(playerMachineCenter, null);
        pSSMC = new PlayerSubStateMachineCenter(this, miniStateArray, playerMachineCenter);
    }

    public override void EnterState()
    {
        pSSMC.SetPrevCurrState(miniStateArray[0]);
        pSSMC.GetCurrentSubState().EnterState();
    }

    public override void ExitState()
    {
        playerMachineCenter.pFSMTH.ResetInputs();
        pSSMC.GetCurrentSubState().ExitState();
        playerMachineCenter.abilities.grapplePoleBehavior.ExitAction();
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
        pSSMC.PerformSubMachineBehavior();
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
}