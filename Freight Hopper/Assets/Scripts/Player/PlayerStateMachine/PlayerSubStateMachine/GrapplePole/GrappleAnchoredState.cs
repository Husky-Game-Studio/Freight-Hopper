public class GrappleAnchoredState : BasicState
{
    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        // Perform grapple pole behavior
        playerMachine.abilities.grapplePoleBehavior.Grapple(UserInput.Input.Move());
    }
}