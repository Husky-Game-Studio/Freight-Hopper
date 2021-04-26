using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// State Machine help from these url:
// https://www.youtube.com/watch?v=nnrOhb5UdRc

public class PlayerMachineCenter : FiniteStateMachineCenter
{
    // State independent fields
    private bool jumpPressed = false;

    [SerializeField] public Timer jumpHoldingTimer = new Timer(0.5f);
    [SerializeField] public Timer coyoteeTimer = new Timer(0.5f);
    [SerializeField] public Timer jumpBufferTimer = new Timer(0.3f);

    // Player States

    public IdleState idleState = new IdleState();
    public FallState fallState = new FallState();
    public RunState runState = new RunState();

    // If it has a substate use the constructor

    public JumpState jumpState;
    public DoubleJumpState doubleJumpState;
    public GroundPoundState groundPoundState;
    public BurstState burstState;
    public FullStopState fullStopState;
    public UpwardDashState upwardDashState;
    public WallRunState wallRunState;
    public GrapplePoleState grapplePoleState;

    // Input Components
    [HideInInspector] public PlayerAbilities playerAbilities;

    [HideInInspector] public CollisionManagement playerCM;

    public PlayerMachineCenter()
    {
        jumpState = new JumpState(this);
        doubleJumpState = new DoubleJumpState(this);
        groundPoundState = new GroundPoundState(this);
        grapplePoleState = new GrapplePoleState(this);
    }

    public override void OnValidate()
    {
        playerAbilities = GetComponent<PlayerAbilities>();
        playerCM = GetComponent<CollisionManagement>();
    }

    public override void OnEnable()
    {
        currentState = idleState;
        previousState = idleState;
        currentState.SubToListeners(this);
        playerCM.CollisionDataCollected += LateFixedUpdate;

        UserInput.Input.JumpInput += this.JumpButtonPressed;
    }

    public override void OnDisable()
    {
        currentState.UnsubToListeners(this);
        playerCM.CollisionDataCollected -= LateFixedUpdate;

        UserInput.Input.JumpInput -= this.JumpButtonPressed;
    }

    // perform anything that is independent of being in any one single state
    public override void PerformStateIndependentBehaviors()
    {
        if (jumpPressed)
        {
            jumpBufferTimer.ResetTimer();
            jumpPressed = false;
        }
        else
        {
            jumpPressed = false;
            jumpBufferTimer.DeactivateTimer();
        }
        if (jumpBufferTimer.TimerActive())
        {
            jumpBufferTimer.CountDownFixed();
        }
    }

    private void JumpButtonPressed()
    {
        jumpPressed = true;
    }
}