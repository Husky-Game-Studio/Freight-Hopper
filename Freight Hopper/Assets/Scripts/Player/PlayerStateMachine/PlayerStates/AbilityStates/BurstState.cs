public class BurstState : BasicState
{
    private PlayerMachineCenter myPlayerMachineCenter;

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter = playerMachine;

        playerMachine.abilities.burstBehavior.EntryAction();
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter.abilities.burstBehavior.ExitAction();
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        return myPlayerMachineCenter.fallState;
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter.abilities.burstBehavior.Action();
    }

    public bool HasSubStateMachine()
    {
        return false;
    }

    public BasicState GetCurrentSubState()
    {
        return null;
    }

    public BasicState[] GetSubStateArray()
    {
        return null;
    }
}