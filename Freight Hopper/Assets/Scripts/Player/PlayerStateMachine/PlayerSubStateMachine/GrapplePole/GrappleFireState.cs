public class GrappleFireState : BasicState
{
    private bool startOfGrapple = true;

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        startOfGrapple = true;
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        if (playerMachine.playerAbilities.grapplePoleBehavior.Anchored())
        {
            return playerMachine.GetCurrentState().GetSubStateArray()[1];
        }

        return this;
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        // Call animation and mesh generation
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        playerMachine.playerAbilities.movementBehavior.Action();
        if (startOfGrapple)
        {
            playerMachine.playerAbilities.grapplePoleBehavior.EntryAction();
            startOfGrapple = false;
        }
        playerMachine.playerAbilities.grapplePoleBehavior.GrappleTransition();
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