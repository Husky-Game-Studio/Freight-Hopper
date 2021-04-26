public class GroundPoundInitialState : BasicState
{
    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        return playerMachine.GetCurrentState().GetSubStateArray()[1];
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        // Perform grapple pole behavior
        playerMachine.playerAbilities.groundPoundBehavior.EntryAction();
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