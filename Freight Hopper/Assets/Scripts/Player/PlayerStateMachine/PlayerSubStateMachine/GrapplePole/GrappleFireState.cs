public class GrappleFireState : BasicState
{
    private bool startOfGrapple = true;

    public override void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        startOfGrapple = true;
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        if (playerMachine.abilities.grapplePoleBehavior.Anchored())
        {
            return playerMachine.GetCurrentState().GetSubStateArray()[1];
        }

        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        // Call animation and mesh generation
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        playerMachine.abilities.movementBehavior.PlayerMove();
        if (startOfGrapple)
        {
            playerMachine.abilities.grapplePoleBehavior.EntryAction();
            startOfGrapple = false;
        }
        playerMachine.abilities.grapplePoleBehavior.GrappleTransition();
    }
}