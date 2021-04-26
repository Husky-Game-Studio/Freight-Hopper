public class GrapplePoleState : BasicState
{
    private bool grapplePolePressed = false;
    private bool jumpPressed = false;
    private PlayerSubStateMachineCenter pSSMC;
    private PlayerMachineCenter myPlayerMachineCenter;
    private BasicState[] miniStateArray;

    public GrapplePoleState(PlayerMachineCenter myPMC)
    {
        myPlayerMachineCenter = myPMC;
        miniStateArray = new BasicState[2];
        miniStateArray[0] = new GrappleFireState();
        miniStateArray[1] = new GrappleAnchoredState();
        pSSMC = new PlayerSubStateMachineCenter(this, miniStateArray, myPlayerMachineCenter);
    }

    public void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        UserInput.Input.GrappleInput += this.GrapplePolePressed;
        UserInput.Input.JumpInput += this.JumpButtonPressed;
        pSSMC.SetPrevCurrState(miniStateArray[0]);
        pSSMC.GetCurrentSubState().SubToListeners(playerMachine);
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        UserInput.Input.GrappleInput -= this.GrapplePolePressed;
        UserInput.Input.JumpInput -= this.JumpButtonPressed;
        pSSMC.GetCurrentSubState().UnsubToListeners(playerMachine);
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        if (grapplePolePressed ||
            (playerMachine.playerAbilities.grapplePoleBehavior.GrapplePoleBroken() && playerMachine.playerAbilities.grapplePoleBehavior.Anchored()))
        {
            grapplePolePressed = false;
            playerMachine.playerAbilities.grapplePoleBehavior.ExitAction();
            return playerMachine.fallState;
        }

        if (jumpPressed)
        {
            jumpPressed = false;
            playerMachine.playerAbilities.grapplePoleBehavior.ExitAction();
            return playerMachine.jumpState;
        }

        return this;
    }

    private void JumpButtonPressed()
    {
        if (GetCurrentSubState() == miniStateArray[1])
        {
            jumpPressed = true;
        }
    }

    private void GrapplePolePressed()
    {
        grapplePolePressed = true;
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