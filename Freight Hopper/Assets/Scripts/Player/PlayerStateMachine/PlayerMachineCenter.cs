using System.Collections.Generic;
using UnityEngine;
using System;

// State Machine help from these url: https://www.youtube.com/watch?v=nnrOhb5UdRc

public class PlayerMachineCenter : FiniteStateMachineCenter
{
    // Player State Machine Transition Handler
    private PlayerStatesTransitions transitionHandler;

    // Player States
    public MoveState moveState;
    public JumpState jumpState;
    public GroundPoundState groundPoundState;
    public WallRunState wallRunState;

    public MovementBehavior movementBehavior;
    public JumpBehavior jumpBehavior;
    public GroundPoundBehavior groundPoundBehavior;
    public WallRunBehavior wallRunBehavior;

    [SerializeField] private GameObject defaultCrosshair;
    private FirstPersonCamera cameraController;

    [HideInInspector] public CollisionManagement collisionManagement;
    [HideInInspector] public Friction friction;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<FirstPersonCamera>();
        transitionHandler = new PlayerStatesTransitions(this);

        // Default
        List<Func<BasicState>> defaultTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckToMoveState,
            transitionHandler.CheckToJumpState,
            transitionHandler.CheckToDefaultState,
            transitionHandler.CheckToGroundPoundState,
            transitionHandler.CheckToWallRunState,
        };
        defaultState = new DefaultState(this, defaultTransitionsList);

        // Move
        List<Func<BasicState>> moveTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckToDefaultState,
            transitionHandler.CheckToJumpState,
            transitionHandler.CheckToGroundPoundState,
            transitionHandler.CheckToWallRunState,
        };
        moveState = new MoveState(this, moveTransitionsList);

        // Jump
        List<Func<BasicState>> jumpTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckToDefaultState,
            transitionHandler.CheckToGroundPoundState,
            transitionHandler.CheckToWallRunState,
        };
        jumpState = new JumpState(this, jumpTransitionsList);

        // Ground Pound
        List<Func<BasicState>> groundPoundTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckToDefaultState,
            transitionHandler.CheckToJumpState,
        };
        groundPoundState = new GroundPoundState(this, groundPoundTransitionsList);

        // Wall Run
        List<Func<BasicState>> wallRunTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckToDefaultState,
            transitionHandler.CheckToGroundPoundState,
        };
        wallRunState = new WallRunState(this, wallRunTransitionsList);

        // Wall Run Wall Running
        List<Func<BasicState>> wallRunSideWallRunningTransitions = new List<Func<BasicState>>
        {
            transitionHandler.CheckToWallRunWallClimbingState,
            transitionHandler.CheckToWallRunWallJumpState
        };
        wallRunState.GetSubStateArray()[0] = new SideWallRunningState(this, wallRunSideWallRunningTransitions);

        wallRunBehavior.Initialize();
        movementBehavior.Initialize();
        jumpBehavior.Initialize();
        groundPoundBehavior.Initialize();
    }

    public void OnEnable()
    {
        collisionManagement = Player.Instance.modules.collisionManagement;
        friction = Player.Instance.modules.friction;
        RestartFSM();
        collisionManagement.CollisionDataCollected += UpdateLoop;
        collisionManagement.Landed += jumpBehavior.Recharge;
        collisionManagement.Landed += groundPoundBehavior.Recharge;
        LevelController.PlayerRespawned += RestartFSM;
    }

    public void OnDisable()
    {
        currentState.ExitState();
        collisionManagement.CollisionDataCollected -= UpdateLoop;
        transitionHandler.OnDisable();
        LevelController.PlayerRespawned -= RestartFSM;
    }

    protected override void EndLoop()
    {
        transitionHandler.ResetInputs();
    }

    public override void PerformStateIndependentBehaviors()
    {
        JumpBuffer();
        movementBehavior.UpdateMovement();
        if (collisionManagement.IsGrounded.current)
        {
            jumpBehavior.coyoteeTimer.ResetTimer();
            wallRunBehavior.inAirCooldown.ResetTimer();
        }
        else
        {
            jumpBehavior.coyoteeTimer.CountDown(Time.fixedDeltaTime);
            wallRunBehavior.inAirCooldown.CountDown(Time.fixedDeltaTime);
        }

        wallRunBehavior.entryEffectsCooldown.CountDown(Time.fixedDeltaTime);

        if (currentState != wallRunState)
        {
            wallRunBehavior.exitEffectsCooldown.CountDown(Time.fixedDeltaTime);
            wallRunBehavior.coyoteTimer.CountDown(Time.fixedDeltaTime);
            if (!wallRunBehavior.exitEffectsCooldown.TimerActive())
            {
                cameraController.ResetUpAxis();
            }
        }

        if (UserInput.Instance.Move().IsZero() && currentState != groundPoundState)
        {
            friction.ResetFrictionReduction();
        }

    }

    private void JumpBuffer()
    {
        if (transitionHandler.jumpPressed.value)
        {
            jumpBehavior.jumpBufferTimer.ResetTimer();
        }
        if (jumpBehavior.jumpBufferTimer.TimerActive())
        {
            jumpBehavior.jumpBufferTimer.CountDown(Time.fixedDeltaTime);
        }
    }
}