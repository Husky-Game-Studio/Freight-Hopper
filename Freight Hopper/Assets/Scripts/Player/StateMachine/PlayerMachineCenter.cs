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
        currentState.SubToListeners(this);
        collision.CollisionDataCollected += LateFixedUpdate;
    }

    void OnDisable()
    {
        currentState.UnsubToListeners(this);
        collision.CollisionDataCollected -= LateFixedUpdate;
    }


    private void LateFixedUpdate()
    {
        if (previousState != currentState)
        {
            currentState.SubToListeners(this);
            previousState.UnsubToListeners(this);
            previousState = currentState;
        }

        if (currentState != null)
        {
            // entry behavior


            currentState.PerformBehavior(this);



            currentState = currentState.TransitionState(this);

            if (previousState != currentState) { // if the current state changed
            // run old state exit behavior
            // run current state entry behavior
            }

            // exit behavior

            currentStateName = currentState.ToString();

            //previousState = currentState;
        }
        else
        {
            Debug.Log("currentState is null");
        }
    }
}
