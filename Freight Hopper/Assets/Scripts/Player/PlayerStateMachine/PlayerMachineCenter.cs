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
        idleTransitionsList.Add(transitionHandler.CheckToRunState);
        idleTransitionsList.Add(transitionHandler.CheckToJumpState);
        idleTransitionsList.Add(transitionHandler.CheckToFallState);
        idleTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        idleTransitionsList.Add(transitionHandler.CheckToFullStopState);
        idleTransitionsList.Add(transitionHandler.CheckToBurstState);
        idleTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        idleTransitionsList.Add(transitionHandler.CheckToGrapplePoleState);
        idleState = new IdleState(this, idleTransitionsList);

        // Run Transitions
        List<Func<BasicState>> runTransitionsList = new List<Func<BasicState>>();
        runTransitionsList.Add(transitionHandler.CheckToIdleState);
        runTransitionsList.Add(transitionHandler.CheckToJumpState);
        runTransitionsList.Add(transitionHandler.CheckToFallState);
        runTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        runTransitionsList.Add(transitionHandler.CheckToFullStopState);
        runTransitionsList.Add(transitionHandler.CheckToBurstState);
        runTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        runTransitionsList.Add(transitionHandler.CheckToGrapplePoleState);
        runState = new RunState(this, runTransitionsList);

        // Jump Transitions
        List<Func<BasicState>> jumpTransitionsList = new List<Func<BasicState>>();
        jumpTransitionsList.Add(transitionHandler.CheckToFallState);
        jumpTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        jumpTransitionsList.Add(transitionHandler.CheckToFullStopState);
        jumpTransitionsList.Add(transitionHandler.CheckToBurstState);
        jumpTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        jumpTransitionsList.Add(transitionHandler.CheckToWallRunState);
        jumpTransitionsList.Add(transitionHandler.CheckToGrapplePoleState);
        jumpState = new JumpState(this, jumpTransitionsList);

        // Fall Transitions
        List<Func<BasicState>> fallTransitionsList = new List<Func<BasicState>>();
        fallTransitionsList.Add(transitionHandler.CheckToIdleState);
        fallTransitionsList.Add(transitionHandler.CheckToJumpState);
        fallTransitionsList.Add(transitionHandler.CheckToDoubleJumpState);
        fallTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        fallTransitionsList.Add(transitionHandler.CheckToFullStopState);
        fallTransitionsList.Add(transitionHandler.CheckToBurstState);
        fallTransitionsList.Add(transitionHandler.CheckToGrapplePoleState);
        fallTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        fallTransitionsList.Add(transitionHandler.CheckToWallRunState);
        fallState = new FallState(this, fallTransitionsList);

        // Double Jump Transitions
        List<Func<BasicState>> doubleJumpTransitionsList = new List<Func<BasicState>>();
        doubleJumpTransitionsList.Add(transitionHandler.CheckToFallState);
        doubleJumpTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        doubleJumpTransitionsList.Add(transitionHandler.CheckToFullStopState);
        doubleJumpTransitionsList.Add(transitionHandler.CheckToBurstState);
        doubleJumpTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        doubleJumpTransitionsList.Add(transitionHandler.CheckToWallRunState);
        doubleJumpTransitionsList.Add(transitionHandler.CheckToGrapplePoleState);
        doubleJumpTransitionsList.Add(transitionHandler.CheckToWallRunState);
        doubleJumpState = new DoubleJumpState(this, doubleJumpTransitionsList);

        // Full Stop Transitions
        List<Func<BasicState>> fullStopTransitionsList = new List<Func<BasicState>>();
        fullStopTransitionsList.Add(transitionHandler.CheckToFallState);
        fullStopState = new FullStopState(this, fullStopTransitionsList);

        // Grapple Pole Transitions
        List<Func<BasicState>> grapplePoleTransistionsList = new List<Func<BasicState>>();
        grapplePoleTransistionsList.Add(transitionHandler.CheckToFallState);
        grapplePoleTransistionsList.Add(transitionHandler.CheckToJumpState);
        grapplePoleState = new GrapplePoleState(this, grapplePoleTransistionsList);

        // Grapple Pole Anchor Transitions
        List<Func<BasicState>> GrapplePoleAnchorTransitions = new List<Func<BasicState>>();
        GrapplePoleAnchorTransitions.Add(transitionHandler.CheckToGrappleGroundPoundState);
        grapplePoleState.GetSubStateArray()[0] = new GrapplePoleAnchoredState(this, GrapplePoleAnchorTransitions);

        // Ground Pound Transitions
        List<Func<BasicState>> groundPoundTransitionsList = new List<Func<BasicState>>();
        groundPoundTransitionsList.Add(transitionHandler.CheckToFallState);
        groundPoundTransitionsList.Add(transitionHandler.CheckToJumpState);
        groundPoundTransitionsList.Add(transitionHandler.CheckToFullStopState);
        groundPoundTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        groundPoundTransitionsList.Add(transitionHandler.CheckToDoubleJumpState);
        groundPoundTransitionsList.Add(transitionHandler.CheckToGrappleGroundPoundState);
        groundPoundState = new GroundPoundState(this, groundPoundTransitionsList);

        // Upward Dash Transitions
        List<Func<BasicState>> upwardDashTransitionsList = new List<Func<BasicState>>();
        upwardDashTransitionsList.Add(transitionHandler.CheckToFallState);
        upwardDashTransitionsList.Add(transitionHandler.CheckToFullStopState);
        upwardDashTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        upwardDashTransitionsList.Add(transitionHandler.CheckToDoubleJumpState);
        upwardDashTransitionsList.Add(transitionHandler.CheckToGrapplePoleState);
        upwardDashState = new UpwardDashState(this, upwardDashTransitionsList);

        // Wall Run Transisitons
        List<Func<BasicState>> wallRunTransitionsList = new List<Func<BasicState>>();
        wallRunTransitionsList.Add(transitionHandler.CheckToFallState);
        wallRunTransitionsList.Add(transitionHandler.CheckToIdleState);
        wallRunTransitionsList.Add(transitionHandler.CheckToGrapplePoleState);
        wallRunTransitionsList.Add(transitionHandler.CheckToFullStopState);
        wallRunTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        wallRunTransitionsList.Add(transitionHandler.CheckToBurstState);
        wallRunTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        wallRunState = new WallRunState(this, wallRunTransitionsList);

        // Wall Run Wall Running Transitions
        List<Func<BasicState>> wallRunSideWallRunningTransitions = new List<Func<BasicState>>();
        wallRunSideWallRunningTransitions.Add(transitionHandler.CheckToWallRunWallClimbingState);
        wallRunSideWallRunningTransitions.Add(transitionHandler.CheckToWallRunWallJumpState);
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