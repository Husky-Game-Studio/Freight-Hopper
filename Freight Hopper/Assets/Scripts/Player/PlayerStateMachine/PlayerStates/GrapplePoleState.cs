using UnityEngine;

public class GrapplePoleState : BasicState
{
    /// <summary>
    /// W Forward
    /// S Backwards
    /// A Left
    /// D Right
    /// Left Mouse - Fire/Release
    /// - Retract
    /// - Extend
    /// </summary>

    private bool grapplePolePressed = false;

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
        pSSMC.SetPrevCurrState(miniStateArray[0]);
        pSSMC.GetCurrentSubState().SubToListeners(playerMachine);
    }

    public void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        UserInput.Input.GrappleInput -= this.GrapplePolePressed;
        pSSMC.GetCurrentSubState().UnsubToListeners(playerMachine);
    }

    public BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        if (grapplePolePressed)
        {
            grapplePolePressed = false;
            return playerMachine.fallState;
        }

        return this;
    }

    private void GrapplePolePressed()
    {
        grapplePolePressed = true;
    }

    public void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;

        // Perform the SubStateMachine Behavior
        pSSMC.PerformSubMachineBehavior();
    }

    public bool HasSubStateMachine()
    {
        return true;
    }

    public BasicState GetCurrentSubState()
    {
        return pSSMC.GetCurrentSubState(); ;
    }

    public BasicState[] GetSubStateArray()
    {
        return miniStateArray;
    }
}