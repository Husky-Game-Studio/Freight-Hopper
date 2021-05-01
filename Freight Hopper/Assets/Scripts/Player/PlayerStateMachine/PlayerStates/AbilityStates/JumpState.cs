public class JumpState : BasicState
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
        playerMachine.abilities.jumpBehavior.EntryAction();
    }

    public override void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        UserInput.Instance.JumpInputCanceled -= this.ReleasedJumpButtonPressed;
        UserInput.Instance.GrappleInput -= this.GrappleButtonPressed;

        // deactivate jump hold timer
        myPlayerMachineCenter.jumpHoldingTimer.DeactivateTimer();

        myPlayerMachineCenter.abilities.jumpBehavior.ExitAction();
        releasedJumpPressed = false;
        grapplePressed = false;
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        // Fall
        if (releasedJumpPressed || !myPlayerMachineCenter.jumpHoldingTimer.TimerActive())
        {
            return myPlayerMachineCenter.fallState;
        }
        // Grapple pole
        if (grapplePressed && !myPlayerMachineCenter.abilities.grapplePoleBehavior.Consumed && myPlayerMachineCenter.abilities.grapplePoleBehavior.Unlocked)
        {
            return myPlayerMachineCenter.grapplePoleState;
        }

        // Jump
        else
        {
            return this;
        }
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        // each fixedupdate the jump button is pressed down, this timer should decrease by that time
        myPlayerMachineCenter.jumpHoldingTimer.CountDownFixed();

        myPlayerMachineCenter.abilities.jumpBehavior.Action();
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