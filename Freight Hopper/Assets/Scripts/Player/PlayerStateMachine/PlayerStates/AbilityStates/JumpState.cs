public class JumpState : BasicState
{
    private bool grapplePressed = false;
    private bool releasedJumpPressed = false;
    private PlayerMachineCenter myPlayerMachineCenter;

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter = playerMachine;
        UserInput.Input.JumpInputCanceled += this.ReleasedJumpButtonPressed;
        UserInput.Input.GrappleInput += this.GrappleButtonPressed;

        // reset jump hold timer
        playerMachine.jumpHoldingTimer.ResetTimer();
        playerMachine.abilities.jumpBehavior.EntryAction();
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        UserInput.Input.JumpInputCanceled -= this.ReleasedJumpButtonPressed;
        UserInput.Input.GrappleInput -= this.GrappleButtonPressed;

        // deactivate jump hold timer
        myPlayerMachineCenter.jumpHoldingTimer.DeactivateTimer();

        myPlayerMachineCenter.abilities.jumpBehavior.ExitAction();
        releasedJumpPressed = false;
        grapplePressed = false;
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        // Fall
        if (releasedJumpPressed || !myPlayerMachineCenter.jumpHoldingTimer.TimerActive())
        {
            myPlayerMachineCenter.jumpHoldingTimer.DeactivateTimer();
            return myPlayerMachineCenter.fallState;
        }
        // Grapple pole
        if (grapplePressed && !myPlayerMachineCenter.abilities.grapplePoleBehavior.IsConsumed)
        {
            return myPlayerMachineCenter.grapplePoleState;
        }

        // Jump
        else
        {
            return this;
        }
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
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