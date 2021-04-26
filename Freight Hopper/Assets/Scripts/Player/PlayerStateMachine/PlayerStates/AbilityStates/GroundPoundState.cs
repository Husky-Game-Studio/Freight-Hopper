public class GroundPoundState : BasicState
{
    private bool groundPoundReleased = false;
    private bool jumpPressed = false;
    private PlayerSubStateMachineCenter pSSMC;
    private PlayerMachineCenter myPlayerMachineCenter;
    private BasicState[] miniStateArray;

    public GroundPoundState(PlayerMachineCenter myPMC)
    {
        myPlayerMachineCenter = myPMC;
        miniStateArray = new BasicState[2];
        miniStateArray[0] = new GroundPoundInitialState();
        miniStateArray[1] = new GroundPoundFall();

        pSSMC = new PlayerSubStateMachineCenter(this, miniStateArray, myPlayerMachineCenter);
    }

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        UserInput.Input.JumpInput += this.JumpButtonPressed;
        UserInput.Input.GroundPoundCanceled += this.GroundPoundButtonReleased;
        pSSMC.SetPrevCurrState(miniStateArray[0]);
        pSSMC.GetCurrentSubState().SubToListeners(playerMachine);
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        UserInput.Input.JumpInput -= this.JumpButtonPressed;
        UserInput.Input.GroundPoundCanceled -= this.GroundPoundButtonReleased;
        pSSMC.GetCurrentSubState().UnsubToListeners(playerMachine);
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        // Fall
        if (groundPoundReleased)
        {
            groundPoundReleased = false;
            playerMachine.playerAbilities.groundPoundBehavior.ExitAction();
            return playerMachine.fallState;
        }
        groundPoundReleased = false;
        // Jump
        if (jumpPressed && !playerMachine.playerAbilities.jumpBehavior.IsConsumed)
        {
            jumpPressed = false;
            playerMachine.playerAbilities.groundPoundBehavior.ExitAction();
            return playerMachine.jumpState;
        }
        // Double Jump
        if (jumpPressed && playerMachine.playerAbilities.jumpBehavior.IsConsumed && !playerMachine.playerAbilities.doubleJumpBehavior.IsConsumed)
        {
            jumpPressed = false;
            playerMachine.playerAbilities.groundPoundBehavior.ExitAction();
            return playerMachine.doubleJumpState;
        }
        // Grapple (SHOULDN'T CANCEL GROUND POUND)
        // Burst
        // Upward Dash

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
        pSSMC.PerformSubMachineBehavior();
    }

    public bool HasSubStateMachine()
    {
        return true;
    }

    public BasicState GetCurrentSubState()
    {
        return pSSMC.GetCurrentSubState();
    }

    public BasicState[] GetSubStateArray()
    {
        return miniStateArray;
    }
}