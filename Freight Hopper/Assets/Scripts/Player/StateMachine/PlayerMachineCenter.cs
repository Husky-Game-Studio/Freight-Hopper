using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// State Machine help from these url:
// https://www.youtube.com/watch?v=nnrOhb5UdRc

public class PlayerMachineCenter : MonoBehaviour
{
    // State machine fields
    [SerializeField] private string currentStateName;
    private BasicState previousState;
    private BasicState currentState;
    [SerializeField] private string currentSubStateName;
    private BasicState currentSubState;
    
    // state independent fields
    private bool jumpPressed = false;
    [SerializeField] public Timer jumpHoldingTimer = new Timer(0.5f);
    [SerializeField] public Timer coyoteeTimer = new Timer(0.5f);
    [SerializeField] public Timer jumpBufferTimer = new Timer(0.3f);
    /*public bool CanCoyotee => coyoteeTimer.TimerActive();
    public bool CanBuffer => jumpBufferTimer.TimerActive();
    public bool IsJumpHolding => jumpHoldingTimer.TimerActive();*/

    // Player States
    public IdleState idleState = new IdleState();
    public RunState runState = new RunState();
    public JumpState jumpState;
    public FallState fallState = new FallState();

    // Input Components
    //public UserInput userInput;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public CollisionManagement collision;

    public PlayerMachineCenter()
    {
        jumpState = new JumpState(this);
    }

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

        UserInput.Input.JumpInput += this.JumpButtonPressed;
    }

    void OnDisable()
    {
        currentState.UnsubToListeners(this);
        collision.CollisionDataCollected -= LateFixedUpdate;

        UserInput.Input.JumpInput -= this.JumpButtonPressed;
    }


    private void LateFixedUpdate()
    {
        this.performStateIndependentBehaviors();

        //if the jump button is pressed, then reset jump buffer timer

        // If current state is a new transisiton, unsub from old listeners, and sub to new ones
        if (previousState != currentState)
        {
            currentState.SubToListeners(this);
            previousState.UnsubToListeners(this);
            previousState = currentState;
        }

        // Perform state behavior
        currentState.PerformBehavior(this);

        // check if the state needs to transition, and return the state it should belong in
        currentState = currentState.TransitionState(this);

        // Debugging
        currentStateName = currentState.ToString();
        if (!currentState.HasSubStateMachine()) {
            currentSubStateName = "No Sub States";
        }
        else
        {
            currentSubState = currentState.GetCurrentSubState();
            currentSubStateName = currentSubState.ToString();
        }
    }

    private void performStateIndependentBehaviors()
    {
        if (jumpPressed) {
            // start jumpBuffer timer
            jumpBufferTimer.ResetTimer();
            jumpPressed = false;
        } else
        {
            jumpPressed = false;
            jumpBufferTimer.DeactivateTimer();
        }
        if (jumpBufferTimer.TimerActive()) {
            jumpBufferTimer.CountDownFixed();
        }
    }

    public BasicState GetCurrentState() {
        return this.currentState;
    }

    private void JumpButtonPressed()
    {
        jumpPressed = true;
    }

    public BasicState getPrevState() {
        return previousState;
    }
}
