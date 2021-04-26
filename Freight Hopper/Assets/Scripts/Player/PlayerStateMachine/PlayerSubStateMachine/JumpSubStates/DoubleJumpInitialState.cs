public class DoubleJumpInitialState : JumpInitialState
{
    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        playerMachine.playerAbilities.doubleJumpBehavior.EntryAction();
        playerMachine.playerAbilities.movementBehavior.Action();
    }
}