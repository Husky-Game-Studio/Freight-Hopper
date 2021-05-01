public class DoubleJumpState : BasicState
{
    private bool grapplePressed = false;
    private bool releasedJumpPressed = false;
    private PlayerMachineCenter myPlayerMachineCenter;

    public override void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter = playerMachine;
        UserInput.Instance.JumpInputCanceled += this.ReleasedJumpButtonPressed;
        UserInput.Instance.GrappleInput += this.GrappleButtonPressed;

        // reset jump hold timer
        playerMachine.jumpHoldingTimer.ResetTimer();
        myPlayerMachineCenter.abilities.doubleJumpBehavior.EntryAction();
    }

    public override void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        UserInput.Instance.JumpInputCanceled -= this.ReleasedJumpButtonPressed;
        UserInput.Instance.GrappleInput -= this.GrappleButtonPressed;

        // deactivate jump hold timer
        myPlayerMachineCenter.jumpHoldingTimer.DeactivateTimer();
        myPlayerMachineCenter.abilities.doubleJumpBehavior.ExitAction();
        releasedJumpPressed = false;
        grapplePressed = false;
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        // Fall
        if (releasedJumpPressed || !playerMachine.jumpHoldingTimer.TimerActive())
        {
            playerMachine.jumpHoldingTimer.DeactivateTimer();
            return playerMachine.fallState;
        }
        // Grapple pole
        if (grapplePressed && !playerMachine.abilities.jumpBehavior.Consumed && playerMachine.abilities.grapplePoleBehavior.Unlocked)
        {
            return playerMachine.grapplePoleState;
        }

        // Double Jump
        releasedJumpPressed = false;
        grapplePressed = false;
        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        // each fixedupdate the jump button is pressed down, this timer should decrease by that time
        myPlayerMachineCenter.jumpHoldingTimer.CountDownFixed();
        myPlayerMachineCenter.abilities.doubleJumpBehavior.Action();
    }

    private void ReleasedJumpButtonPressed()
    {
        releasedJumpPressed = true;
    }

    private void GrappleButtonPressed()
    {
        grapplePressed = true;
    }
}