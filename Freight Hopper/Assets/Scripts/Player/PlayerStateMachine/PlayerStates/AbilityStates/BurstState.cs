using UnityEngine;

public class BurstState : BasicState
{
    private PlayerMachineCenter myPlayerMachineCenter;

    public override void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter = playerMachine;

        playerMachine.abilities.burstBehavior.EntryAction();
    }

    public override void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter.abilities.burstBehavior.ExitAction();
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        return myPlayerMachineCenter.fallState;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter.abilities.burstBehavior.Action();
    }
}