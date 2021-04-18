using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// State Machine help from these url:
// https://www.youtube.com/watch?v=nnrOhb5UdRc

public class PlayerMachineCenter : FiniteStateMachineCenter
{
    // state independent fields
    private bool jumpPressed = false;
    [SerializeField] public Timer jumpHoldingTimer = new Timer(0.5f);
    [SerializeField] public Timer coyoteeTimer = new Timer(0.5f);
    [SerializeField] public Timer jumpBufferTimer = new Timer(0.3f);
    
    // Player States
    public IdleState idleState = new IdleState();
    public RunState runState = new RunState();
    public JumpState jumpState;
    public FallState fallState = new FallState();

    // Input Components
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public CollisionManagement collision;

    public PlayerMachineCenter()
    {
        jumpState = new JumpState(this);
    }

    public override void OnValidate()
    {
        playerMovement = GetComponent<PlayerMovement>();
        collision = GetComponent<CollisionManagement>();
    }

    public override void OnEnable()
    {
        currentState = idleState;
        previousState = idleState;
        currentState.SubToListeners(this);
        collision.CollisionDataCollected += LateFixedUpdate;

        UserInput.Input.JumpInput += this.JumpButtonPressed;
    }

    public override void OnDisable()
    {
        currentState.UnsubToListeners(this);
        collision.CollisionDataCollected -= LateFixedUpdate;

        UserInput.Input.JumpInput -= this.JumpButtonPressed;
    }

    // perform anything that is independent of being in any one single state
    public override void PerformStateIndependentBehaviors()
    {
        if (jumpPressed) {
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

    private void JumpButtonPressed()
    {
        jumpPressed = true;
    }
}
