public class FullStopState : BasicState
{
    private PlayerMachineCenter myPlayerMachineCenter;

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter = playerMachine;

        playerMachine.abilities.fullstopBehavior.EntryAction();
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter.abilities.fullstopBehavior.ExitAction();
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        return myPlayerMachineCenter.fallState;
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter.abilities.fullstopBehavior.Action();
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