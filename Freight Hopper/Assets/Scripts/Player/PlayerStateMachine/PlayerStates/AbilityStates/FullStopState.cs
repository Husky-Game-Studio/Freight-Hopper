public class FullStopState : BasicState
{
    private PlayerMachineCenter myPlayerMachineCenter;

    public override void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter = playerMachine;

        playerMachine.abilities.fullstopBehavior.EntryAction();
    }

    public override void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter.abilities.fullstopBehavior.ExitAction();
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        if (myPlayerMachineCenter.abilities.fullstopBehavior.FullStopFinished())
        {
            return myPlayerMachineCenter.fallState;
        }

        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter.abilities.fullstopBehavior.Action();
    }
}