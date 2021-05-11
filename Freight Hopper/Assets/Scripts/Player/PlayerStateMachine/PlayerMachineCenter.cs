using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// State Machine help from these url:
// https://www.youtube.com/watch?v=nnrOhb5UdRc

public class PlayerMachineCenter : FiniteStateMachineCenter
{
    // Player State Machine Transition Handler
    public PlayerStatesTransitions transitionHandler;

    // State independent fields
    private bool grappleFiring;

    // Player States

    public IdleState idleState;
    public FallState fallState;
    public RunState runState;

    public JumpState jumpState;
    public DoubleJumpState doubleJumpState;
    public GroundPoundState groundPoundState;
    public BurstState burstState;
    public FullStopState fullStopState;
    public UpwardDashState upwardDashState;
    public WallRunState wallRunState;
    public GrapplePoleState grapplePoleState;

    // Input Components
    [HideInInspector] public PlayerAbilities abilities;

    [HideInInspector] public CollisionManagement playerCM;

    private void Awake()
    {
        transitionHandler = new PlayerStatesTransitions(this);

        // Idle Transitions
        List<Func<BasicState>> idleTransitionsList = new List<Func<BasicState>>();
        idleTransitionsList.Add(transitionHandler.checkToRunState);
        idleTransitionsList.Add(transitionHandler.checkToJumpState);
        idleTransitionsList.Add(transitionHandler.checkToFallState);
        idleTransitionsList.Add(transitionHandler.checkToGroundPoundState);
        idleTransitionsList.Add(transitionHandler.checkToFullStopState);
        idleTransitionsList.Add(transitionHandler.checkToBurstState);
        idleTransitionsList.Add(transitionHandler.checkToUpwardDashState);
        idleTransitionsList.Add(transitionHandler.checkToGrapplePoleState);
        idleState = new IdleState(this, idleTransitionsList);

        // Run Transitions
        List<Func<BasicState>> runTransitionsList = new List<Func<BasicState>>();
        runTransitionsList.Add(transitionHandler.checkToIdleState);
        runTransitionsList.Add(transitionHandler.checkToJumpState);
        runTransitionsList.Add(transitionHandler.checkToFallState);
        runTransitionsList.Add(transitionHandler.checkToGroundPoundState);
        runTransitionsList.Add(transitionHandler.checkToFullStopState);
        runTransitionsList.Add(transitionHandler.checkToBurstState);
        runTransitionsList.Add(transitionHandler.checkToUpwardDashState);
        runTransitionsList.Add(transitionHandler.checkToGrapplePoleState);
        runState = new RunState(this, runTransitionsList);

        // Jump Transitions
        List<Func<BasicState>> jumpTransitionsList = new List<Func<BasicState>>();
        jumpTransitionsList.Add(transitionHandler.checkToFallState);
        jumpTransitionsList.Add(transitionHandler.checkToGroundPoundState);
        jumpTransitionsList.Add(transitionHandler.checkToFullStopState);
        jumpTransitionsList.Add(transitionHandler.checkToBurstState);
        jumpTransitionsList.Add(transitionHandler.checkToUpwardDashState);
        jumpTransitionsList.Add(transitionHandler.checkToWallRunState);
        jumpTransitionsList.Add(transitionHandler.checkToGrapplePoleState);
        jumpState = new JumpState(this, jumpTransitionsList);

        // Fall Transitions
        List<Func<BasicState>> fallTransitionsList = new List<Func<BasicState>>();
        fallTransitionsList.Add(transitionHandler.checkToIdleState);
        fallTransitionsList.Add(transitionHandler.checkToJumpState);
        fallTransitionsList.Add(transitionHandler.checkToDoubleJumpState);
        fallTransitionsList.Add(transitionHandler.checkToGroundPoundState);
        fallTransitionsList.Add(transitionHandler.checkToFullStopState);
        fallTransitionsList.Add(transitionHandler.checkToBurstState);
        fallTransitionsList.Add(transitionHandler.checkToGrapplePoleState);
        fallTransitionsList.Add(transitionHandler.checkToUpwardDashState);
        fallTransitionsList.Add(transitionHandler.checkToWallRunState);
        fallState = new FallState(this, fallTransitionsList);

        // Double Jump Transitions
        List<Func<BasicState>> doubleJumpTransitionsList = new List<Func<BasicState>>();
        doubleJumpTransitionsList.Add(transitionHandler.checkToFallState);
        doubleJumpTransitionsList.Add(transitionHandler.checkToGroundPoundState);
        doubleJumpTransitionsList.Add(transitionHandler.checkToFullStopState);
        doubleJumpTransitionsList.Add(transitionHandler.checkToBurstState);
        doubleJumpTransitionsList.Add(transitionHandler.checkToUpwardDashState);
        doubleJumpTransitionsList.Add(transitionHandler.checkToWallRunState);
        doubleJumpTransitionsList.Add(transitionHandler.checkToGrapplePoleState);
        doubleJumpTransitionsList.Add(transitionHandler.checkToWallRunState);
        doubleJumpState = new DoubleJumpState(this, doubleJumpTransitionsList);

        // Full Stop Transitions
        List<Func<BasicState>> fullStopTransitionsList = new List<Func<BasicState>>();
        fullStopTransitionsList.Add(transitionHandler.checkToFallState);
        fullStopState = new FullStopState(this, fullStopTransitionsList);

        // Grapple Pole Transitions
        List<Func<BasicState>> grapplePoleTransistionsList = new List<Func<BasicState>>();
        grapplePoleTransistionsList.Add(transitionHandler.checkToFallState);
        grapplePoleTransistionsList.Add(transitionHandler.checkToJumpState);
        grapplePoleState = new GrapplePoleState(this, grapplePoleTransistionsList);

        // Grapple Pole Anchor Transitions
        List<Func<BasicState>> GrapplePoleAnchorTransitions = new List<Func<BasicState>>();
        GrapplePoleAnchorTransitions.Add(transitionHandler.checkToGrappleGroundPoundState);
        grapplePoleState.GetSubStateArray()[0] = new GrapplePoleAnchoredState(this, GrapplePoleAnchorTransitions);

        // Ground Pound Transitions
        List<Func<BasicState>> groundPoundTransitionsList = new List<Func<BasicState>>();
        groundPoundTransitionsList.Add(transitionHandler.checkToFallState);
        groundPoundTransitionsList.Add(transitionHandler.checkToJumpState);
        groundPoundTransitionsList.Add(transitionHandler.checkToFullStopState);
        groundPoundTransitionsList.Add(transitionHandler.checkToUpwardDashState);
        groundPoundTransitionsList.Add(transitionHandler.checkToDoubleJumpState);
        groundPoundTransitionsList.Add(transitionHandler.checkToGrappleGroundPoundState);
        groundPoundState = new GroundPoundState(this, groundPoundTransitionsList);

        // Upward Dash Transitions
        List<Func<BasicState>> upwardDashTransitionsList = new List<Func<BasicState>>();
        upwardDashTransitionsList.Add(transitionHandler.checkToFallState);
        upwardDashTransitionsList.Add(transitionHandler.checkToFullStopState);
        upwardDashTransitionsList.Add(transitionHandler.checkToGroundPoundState);
        upwardDashTransitionsList.Add(transitionHandler.checkToDoubleJumpState);
        upwardDashTransitionsList.Add(transitionHandler.checkToGrapplePoleState);
        upwardDashState = new UpwardDashState(this, upwardDashTransitionsList);

        // Wall Run Transisitons
        List<Func<BasicState>> wallRunTransitionsList = new List<Func<BasicState>>();
        wallRunTransitionsList.Add(transitionHandler.checkToFallState);
        wallRunTransitionsList.Add(transitionHandler.checkToIdleState);
        wallRunTransitionsList.Add(transitionHandler.checkToGrapplePoleState);
        wallRunTransitionsList.Add(transitionHandler.checkToFullStopState);
        wallRunTransitionsList.Add(transitionHandler.checkToGroundPoundState);
        wallRunTransitionsList.Add(transitionHandler.checkToBurstState);
        wallRunTransitionsList.Add(transitionHandler.checkToUpwardDashState);
        wallRunState = new WallRunState(this, wallRunTransitionsList);

        // Wall Run Wall Running Transitions
        List<Func<BasicState>> wallRunSideWallRunningTransitions = new List<Func<BasicState>>();
        wallRunSideWallRunningTransitions.Add(transitionHandler.checkToWallRunWallClimbingState);
        wallRunSideWallRunningTransitions.Add(transitionHandler.checkToWallRunWallJumpState);
        wallRunState.GetSubStateArray()[0] = new SideWallRunningState(this, wallRunSideWallRunningTransitions);

        // Burst Transitions
        burstState = new BurstState(this, null);
    }

    public override void OnValidate()
    {
        abilities = GetComponent<PlayerAbilities>();
        playerCM = GetComponent<CollisionManagement>();
    }

    private void PlayerSpawned()
    {
        currentState.ExitState();
        currentState = idleState;
        previousState = idleState;
        currentState.EnterState();
    }

    public override void OnEnable()
    {
        currentState = idleState;
        previousState = idleState;
        currentState.EnterState();
        playerCM.CollisionDataCollected += LateFixedUpdate;

        LevelController.PlayerRespawned += PlayerSpawned;
    }

    public override void OnDisable()
    {
        currentState.ExitState();
        playerCM.CollisionDataCollected -= LateFixedUpdate;

        LevelController.PlayerRespawned -= PlayerSpawned;
    }

    protected override void EndLoop()
    {
        transitionHandler.ResetInputs();
    }

    // perform anything that is independent of being in any one single state
    public override void PerformStateIndependentBehaviors()
    {
        JumpBuffer();
        GrappleFiring();
    }

    private void JumpBuffer()
    {
        if (transitionHandler.jumpPressed.value)
        {
            abilities.jumpBehavior.jumpBufferTimer.ResetTimer();
        }
        if (abilities.jumpBehavior.jumpBufferTimer.TimerActive())
        {
            abilities.jumpBehavior.jumpBufferTimer.CountDownFixed();
        }
    }

    private void GrappleFiring()
    {
        if (transitionHandler.grapplePressed.value && abilities.grapplePoleBehavior.UnlockedAndReady && previousState.GetType() != typeof(GrapplePoleState))
        {
            if (grappleFiring)
            {
                abilities.grapplePoleBehavior.ResetPole();
                grappleFiring = false;
            }
            else
            {
                abilities.grapplePoleBehavior.EntryAction();
                grappleFiring = true;
            }
        }
        if (grappleFiring)
        {
            if (abilities.grapplePoleBehavior.IsAnchored())
            {
                grappleFiring = false;
            }
            else
            {
                abilities.grapplePoleBehavior.GrappleTransition();
            }
        }
    }
}