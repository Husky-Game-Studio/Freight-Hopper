using System.Collections.Generic;
using System;

public class GrapplePoleState : PlayerState
{
    private PlayerSubStateMachineCenter pSSMC;
    private BasicState[] miniStateArray;
    private bool transitioningFromGroundPound = false;

    public void TransitioningFromGroundPound() => transitioningFromGroundPound = true;

    public GrapplePoleState(PlayerMachineCenter playerMachineCenter, List<Func<BasicState>> myTransitions) : base(playerMachineCenter, myTransitions)
    {
        this.stateTransitions = myTransitions;

        miniStateArray = new BasicState[2];
        miniStateArray[0] = null;
        miniStateArray[1] = new GrappleGroundPoundState(playerMachineCenter, myTransitions);
        pSSMC = new PlayerSubStateMachineCenter(this, miniStateArray, playerMachineCenter);
    }

    public override void EnterState()
    {
        if (transitioningFromGroundPound)
        {
            pSSMC.SetPrevCurrState(miniStateArray[1]);
            transitioningFromGroundPound = false;
        }
        else
        {
            pSSMC.SetPrevCurrState(miniStateArray[0]);
        }

        pSSMC.GetCurrentSubState().EnterState();
    }

    public override void ExitState()
    {
        pSSMC.GetCurrentSubState().ExitState();
        playerMachineCenter.abilities.grapplePoleBehavior.ExitAction();
    }

    public override BasicState TransitionState()
    {
        BasicState state = CheckTransitions();

        return state;
    }

    public override void PerformBehavior()
    {
        pSSMC.PerformSubMachineBehavior();
        playerMachineCenter.abilities.grapplePoleBehavior.Grapple(UserInput.Instance.Move());
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