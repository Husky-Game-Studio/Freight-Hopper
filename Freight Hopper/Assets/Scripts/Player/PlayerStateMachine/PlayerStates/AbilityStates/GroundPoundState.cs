public class GroundPoundState : BasicState
{
    private bool groundPoundReleased = false;
    private bool jumpPressed = false;
    private PlayerMachineCenter myPlayerMachineCenter;

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        myPlayerMachineCenter = playerMachine;
        UserInput.Input.JumpInput += this.JumpButtonPressed;
        UserInput.Input.GroundPoundCanceled += this.GroundPoundButtonReleased;

        playerMachine.abilities.groundPoundBehavior.EntryAction();
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        UserInput.Input.JumpInput -= this.JumpButtonPressed;
        UserInput.Input.GroundPoundCanceled -= this.GroundPoundButtonReleased;

        playerMachine.abilities.groundPoundBehavior.ExitAction();
        groundPoundReleased = false;
        jumpPressed = false;
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        // Fall
        if (groundPoundReleased)
        {
            return playerMachine.fallState;
        }

        // Jump
        if (jumpPressed && !playerMachine.abilities.jumpBehavior.Consumed && playerMachine.abilities.jumpBehavior.Unlocked)
        {
            return playerMachine.jumpState;
        }
        // Double Jump
        if (jumpPressed && playerMachine.abilities.jumpBehavior.Consumed && !playerMachine.abilities.doubleJumpBehavior.Consumed && playerMachine.abilities.doubleJumpBehavior.Unlocked)
        {
            return playerMachine.doubleJumpState;
        }
        // Grapple (SHOULDN'T CANCEL GROUND POUND)
        // Burst
        // Upward Dash
        groundPoundReleased = false;
        jumpPressed = false;
        return this;
    }

    private void GroundPoundButtonReleased()
    {
        groundPoundReleased = true;
    }

    private void JumpButtonPressed()
    {
        jumpPressed = true;
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        myPlayerMachineCenter.abilities.groundPoundBehavior.Action();
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