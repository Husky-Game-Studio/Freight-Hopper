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

    public JumpState jumpState = new JumpState();
    public DoubleJumpState doubleJumpState = new DoubleJumpState();
    public GroundPoundState groundPoundState = new GroundPoundState();
    public BurstState burstState = new BurstState();
    public FullStopState fullStopState = new FullStopState();
    public UpwardDashState upwardDashState = new UpwardDashState();
    public WallRunState wallRunState;
    public GrapplePoleState grapplePoleState;

    // Input Components
    [HideInInspector] public PlayerAbilities abilities;

    [HideInInspector] public CollisionManagement playerCM;

    public PlayerMachineCenter()
    {
        wallRunState = new WallRunState(this);
        grapplePoleState = new GrapplePoleState(this);
    }

    public override void OnValidate()
    {
        abilities = GetComponent<PlayerAbilities>();
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