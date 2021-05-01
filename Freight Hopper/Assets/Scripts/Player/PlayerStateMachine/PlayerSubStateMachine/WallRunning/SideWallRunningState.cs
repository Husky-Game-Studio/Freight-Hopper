using UnityEngine;

public class SideWallRunningState : BasicState
{
    private bool jumpPressed = false;

    public override void SubToListeners(FiniteStateMachineCenter machineCenter)
    {
        UserInput.Instance.JumpInput += this.JumpButtonPressed;
        Debug.Log("entered side wall Jump State");
    }

    public override void UnsubToListeners(FiniteStateMachineCenter machineCenter)
    {
        UserInput.Instance.JumpInput -= this.JumpButtonPressed;
        jumpPressed = false;
        Debug.Log("exited side wall Jump State");
    }

    public override BasicState TransitionState(FiniteStateMachineCenter machineCenter)
    {
        PlayerMachineCenter playerMachine = (PlayerMachineCenter)machineCenter;
        bool[] status = playerMachine.abilities.wallRunBehavior.CheckWalls();

        // Wall Climb
        if (status[0] && !status[1] && !status[3] && !playerMachine.abilities.wallRunBehavior.Consumed)
        {
            Debug.Log("Wall climbed");
            return playerMachine.GetCurrentState().GetSubStateArray()[1];
        }
        // Wall Jump
        if (jumpPressed)
        {
            Debug.Log("Jumped");
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
        Debug.Log("jump button pressed");
        jumpPressed = true;
    }
}