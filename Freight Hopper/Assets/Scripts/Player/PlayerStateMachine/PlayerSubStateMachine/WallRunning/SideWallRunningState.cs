using UnityEngine;

public class SideWallRunningState : BasicState
{
    private bool jumpPressed = false;

    public override void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        UserInput.Instance.JumpInput += this.JumpButtonPressed;
    }

    public override void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        UserInput.Instance.JumpInput -= this.JumpButtonPressed;
        jumpPressed = false;
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        bool[] status = playerMachine.abilities.wallRunBehavior.CheckWalls();

        // Wall Climb
        if (status[0] && !status[1] && !status[3] && !playerMachine.abilities.wallRunBehavior.Consumed)
        {
            return playerMachine.GetCurrentState().GetSubStateArray()[1];
        }
        // Wall Jump
        if (jumpPressed)
        {
            return playerMachine.GetCurrentState().GetSubStateArray()[2];
        }
        jumpPressed = false;
        return this;
    }

    public override void PerformBehavior(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        bool[] status = playerMachine.abilities.wallRunBehavior.CheckWalls();
        if (status[1])
        {
            playerMachine.abilities.wallRunBehavior.RightWallRun();
        }
        if (status[3])
        {
            playerMachine.abilities.wallRunBehavior.LeftWallRun();
        }
    }

    private void JumpButtonPressed()
    {
        jumpPressed = true;
    }
}