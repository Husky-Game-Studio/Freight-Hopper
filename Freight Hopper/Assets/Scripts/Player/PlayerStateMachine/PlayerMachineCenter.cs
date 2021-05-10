using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// State Machine help from these url:
// https://www.youtube.com/watch?v=nnrOhb5UdRc

public class PlayerMachineCenter : FiniteStateMachineCenter
{
    // Player State Machine Transition Handler
    public PlayerStatesTransitions pFSMTH;

    // State independent fields

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
        pFSMTH = new PlayerStatesTransitions(this);

        // Idle Transitions
        List<Func<BasicState>> idleTransitionsList = new List<Func<BasicState>>();
        idleTransitionsList.Add(pFSMTH.checkToRunState);
        idleTransitionsList.Add(pFSMTH.checkToJumpState);
        idleTransitionsList.Add(pFSMTH.checkToFallState);
        idleTransitionsList.Add(pFSMTH.checkToGroundPoundState);
        idleTransitionsList.Add(pFSMTH.checkToFullStopState);
        idleTransitionsList.Add(pFSMTH.checkToBurstState);
        idleTransitionsList.Add(pFSMTH.checkToUpwardDashState);
        idleTransitionsList.Add(pFSMTH.checkToGrapplePoleState);
        idleState = new IdleState(this, idleTransitionsList);

        // Run Transitions
        List<Func<BasicState>> runTransitionsList = new List<Func<BasicState>>();
        runTransitionsList.Add(pFSMTH.checkToIdleState);
        runTransitionsList.Add(pFSMTH.checkToJumpState);
        runTransitionsList.Add(pFSMTH.checkToFallState);
        runTransitionsList.Add(pFSMTH.checkToGroundPoundState);
        runTransitionsList.Add(pFSMTH.checkToFullStopState);
        runTransitionsList.Add(pFSMTH.checkToBurstState);
        runTransitionsList.Add(pFSMTH.checkToUpwardDashState);
        runTransitionsList.Add(pFSMTH.checkToGrapplePoleState);
        runState = new RunState(this, runTransitionsList);

        // Jump Transitions
        List<Func<BasicState>> jumpTransitionsList = new List<Func<BasicState>>();
        jumpTransitionsList.Add(pFSMTH.checkToFallState);
        jumpTransitionsList.Add(pFSMTH.checkToGrapplePoleState);
        jumpState = new JumpState(this, jumpTransitionsList);

        // Fall Transitions
        List<Func<BasicState>> fallTransitionsList = new List<Func<BasicState>>();
        fallTransitionsList.Add(pFSMTH.checkToIdleState);
        fallTransitionsList.Add(pFSMTH.checkToJumpState);
        fallTransitionsList.Add(pFSMTH.checkToDoubleJumpState);
        fallTransitionsList.Add(pFSMTH.checkToGroundPoundState);
        fallTransitionsList.Add(pFSMTH.checkToFullStopState);
        fallTransitionsList.Add(pFSMTH.checkToBurstState);
        fallTransitionsList.Add(pFSMTH.checkToGrapplePoleState);
        fallTransitionsList.Add(pFSMTH.checkToUpwardDashState);
        fallTransitionsList.Add(pFSMTH.checkToWallRunState);
        fallState = new FallState(this, fallTransitionsList);

        // Double Jump Transitions
        List<Func<BasicState>> doubleJumpTransitionsList = new List<Func<BasicState>>();
        doubleJumpTransitionsList.Add(pFSMTH.checkToFallState);
        doubleJumpTransitionsList.Add(pFSMTH.checkToGrapplePoleState);
        doubleJumpState = new DoubleJumpState(this, doubleJumpTransitionsList);

        // Full Stop Transitions
        List<Func<BasicState>> fullStopTransitionsList = new List<Func<BasicState>>();
        fullStopTransitionsList.Add(pFSMTH.checkToFallState);
        fullStopState = new FullStopState(this, fullStopTransitionsList);

        // Grapple Pole Transitions
        List<Func<BasicState>> grapplePoleTransistionsList = new List<Func<BasicState>>();
        grapplePoleTransistionsList.Add(pFSMTH.checkToFallState);
        grapplePoleTransistionsList.Add(pFSMTH.checkToJumpState);
        grapplePoleState = new GrapplePoleState(this, grapplePoleTransistionsList);

        // Gapple Pole Grapple Fire Transitions
        List<Func<BasicState>> grapplePoleGrappleFireTransitionsList = new List<Func<BasicState>>();
        grapplePoleGrappleFireTransitionsList.Add(pFSMTH.checkToGrapplePoleAnchoredState);
        grapplePoleState.GetSubStateArray()[0] = new GrappleFireState(this, grapplePoleGrappleFireTransitionsList);

        // Ground Pound Transitions
        List<Func<BasicState>> groundPoundTransitionsList = new List<Func<BasicState>>();
        groundPoundTransitionsList.Add(pFSMTH.checkToFallState);
        groundPoundTransitionsList.Add(pFSMTH.checkToJumpState);
        groundPoundTransitionsList.Add(pFSMTH.checkToDoubleJumpState);
        groundPoundState = new GroundPoundState(this, groundPoundTransitionsList);

        // Upward Dash Transitions
        List<Func<BasicState>> upwardDashTransitionsList = new List<Func<BasicState>>();
        upwardDashTransitionsList.Add(pFSMTH.checkToFallState);
        upwardDashTransitionsList.Add(pFSMTH.checkToGrapplePoleState);
        upwardDashState = new UpwardDashState(this, upwardDashTransitionsList);

        // Wall Run Transisitons
        List<Func<BasicState>> wallRunTransitionsList = new List<Func<BasicState>>();
        wallRunTransitionsList.Add(pFSMTH.checkToFallState);
        wallRunTransitionsList.Add(pFSMTH.checkToIdleState);
        wallRunState = new WallRunState(this, wallRunTransitionsList);

        // Wall Run Wall Running Transitions
        List<Func<BasicState>> wallRunSideWallRunningTransitions = new List<Func<BasicState>>();
        wallRunSideWallRunningTransitions.Add(pFSMTH.checkToWallRunWallClimbingState);
        wallRunSideWallRunningTransitions.Add(pFSMTH.checkToWallRunWallJumpState);
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

    // perform anything that is independent of being in any one single state
    public override void PerformStateIndependentBehaviors()
    {
        if (pFSMTH.jumpPressed)
        {
            abilities.jumpBehavior.jumpBufferTimer.ResetTimer();
        }
        if (abilities.jumpBehavior.jumpBufferTimer.TimerActive())
        {
            abilities.jumpBehavior.jumpBufferTimer.CountDownFixed();
        }
    }
}