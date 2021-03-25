using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// State Machine help from these url:
// https://www.youtube.com/watch?v=nnrOhb5UdRc

public class PlayerMachineCenter : MonoBehaviour
{
    [SerializeField]
    private string currentStateName;
    private BasicState previousState;
    private BasicState currentState;

    // Player States
    public IdleState idleState = new IdleState();
    public RunState runState = new RunState();
    public JumpState jumpState = new JumpState();
    public FallState fallState = new FallState();

    // Input Components
    //public UserInput userInput;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public CollisionManagement collision;

    private void OnValidate()
    {
        //public UserInput userInput;
        playerMovement = GetComponent<PlayerMovement>();
        collision = GetComponent<CollisionManagement>();
    }

    private void OnEnable()
    {
        currentState = idleState;
        previousState = idleState;
        currentState.subToListeners(this);
    }

    private void Update()
    {
        if (previousState != currentState) {
            currentState.subToListeners(this);
            previousState.unsubToListeners(this);
            previousState = currentState;
        }

        if (currentState != null && currentState != fallState)
        {
            currentState = currentState.DoState(this);
            currentStateName = currentState.ToString();

            //previousState = currentState;
        }
        else {
            Debug.Log("currentState is null");
        }

        
    }

    private void FixedUpdate()
    {
        /*if (previousState != currentState)
        {
            currentState.subToListeners();
            previousState.unsubToListeners();
            previousState = currentState;
        }*/

        if (currentState == fallState) {
            currentState = currentState.DoState(this);
            currentStateName = currentState.ToString();
            //previousState = currentState;
        }
    }
}
