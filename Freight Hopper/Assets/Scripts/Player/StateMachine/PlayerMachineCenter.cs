using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// State Machine help from these url:
// https://www.youtube.com/watch?v=nnrOhb5UdRc

public class PlayerMachineCenter : MonoBehaviour
{
    [SerializeField]
    private string currentStateName;
    private BasicState currentState;

    // Player States
    public IdleState idleState = new IdleState();
    public RunState runState = new RunState();
    public JumpState jumpState = new JumpState();
    public FallState fallState = new FallState();

    // Input Components
    public UserInput userInput;
    public PlayerMovement playerMovement;

    private void OnEnable()
    {
        currentState = idleState;
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState = currentState.DoState(this);
            currentStateName = currentState.ToString();
        }
        else {
            Debug.Log("currentState is null");
        }
    }
}
