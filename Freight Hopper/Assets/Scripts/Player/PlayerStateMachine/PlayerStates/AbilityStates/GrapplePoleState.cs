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

    public override void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        UserInput.Instance.GrappleInput += this.GrapplePolePressed;
        UserInput.Instance.JumpInput += this.JumpButtonPressed;
        pSSMC.SetPrevCurrState(miniStateArray[0]);
        pSSMC.GetCurrentSubState().SubToListeners(playerMachine);
    }

    public override void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        UserInput.Instance.GrappleInput -= this.GrapplePolePressed;
        UserInput.Instance.JumpInput -= this.JumpButtonPressed;

        grapplePolePressed = false;
        jumpPressed = false;
        pSSMC.GetCurrentSubState().UnsubToListeners(playerMachine);
        playerMachine.abilities.grapplePoleBehavior.ExitAction();
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        if (grapplePolePressed ||
            (playerMachine.abilities.grapplePoleBehavior.GrapplePoleBroken() && playerMachine.abilities.grapplePoleBehavior.Anchored()))
        {
            return playerMachine.fallState;
        }

        if (jumpPressed)
        {
            return playerMachine.jumpState;
        }
        grapplePolePressed = false;
        jumpPressed = false;
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

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        pSSMC.PerformSubMachineBehavior();
    }

    public override bool HasSubStateMachine()
    {
        return true;
    }

    public override BasicState GetCurrentSubState()
    {
        return pSSMC.GetCurrentSubState();
    }

    public override BasicState[] GetSubStateArray()
    {
        return miniStateArray;
    }
}