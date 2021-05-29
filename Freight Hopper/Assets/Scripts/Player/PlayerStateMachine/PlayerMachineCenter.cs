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
    public GrapplePoleAnchoredState grapplePoleAnchoredState;
    public GrappleGroundPoundState grapplePoleGroundPoundState;
    public GrappleFullstopState grapplePoleFullStopState;
    public GrappleBurstState grapplePoleBurstState;

    // State independent fields
    [SerializeField, ReadOnly] private bool grappleFiring;

    [SerializeField] public Timer initialGroundPoundBurstCoolDown;

    // Input Components
    [HideInInspector] public PlayerAbilities abilities;

    [HideInInspector] public CollisionManagement playerCM;

    private void Awake()
    {
        transitionHandler = new PlayerStatesTransitions(this);

        // Idle
        List<Func<BasicState>> idleTransitionsList = new List<Func<BasicState>>();
        idleTransitionsList.Add(transitionHandler.CheckToRunState);
        idleTransitionsList.Add(transitionHandler.CheckToJumpState);
        idleTransitionsList.Add(transitionHandler.CheckToFallState);
        idleTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        idleTransitionsList.Add(transitionHandler.CheckToFullStopState);
        idleTransitionsList.Add(transitionHandler.CheckToBurstState);
        idleTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        idleTransitionsList.Add(transitionHandler.CheckToGrapplePoleAnchoredState);
        idleState = new IdleState(this, idleTransitionsList);

        // Run
        List<Func<BasicState>> runTransitionsList = new List<Func<BasicState>>();
        runTransitionsList.Add(transitionHandler.CheckToIdleState);
        runTransitionsList.Add(transitionHandler.CheckToJumpState);
        runTransitionsList.Add(transitionHandler.CheckToFallState);
        runTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        runTransitionsList.Add(transitionHandler.CheckToFullStopState);
        runTransitionsList.Add(transitionHandler.CheckToBurstState);
        runTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        runTransitionsList.Add(transitionHandler.CheckToGrapplePoleAnchoredState);
        runState = new RunState(this, runTransitionsList);

        // Jump
        List<Func<BasicState>> jumpTransitionsList = new List<Func<BasicState>>();
        jumpTransitionsList.Add(transitionHandler.CheckToFallState);
        jumpTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        jumpTransitionsList.Add(transitionHandler.CheckToFullStopState);
        jumpTransitionsList.Add(transitionHandler.CheckToBurstState);
        jumpTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        jumpTransitionsList.Add(transitionHandler.CheckToWallRunState);
        jumpTransitionsList.Add(transitionHandler.CheckToGrapplePoleAnchoredState);
        jumpState = new JumpState(this, jumpTransitionsList);

        // Fall
        List<Func<BasicState>> fallTransitionsList = new List<Func<BasicState>>();
        fallTransitionsList.Add(transitionHandler.CheckToIdleState);
        fallTransitionsList.Add(transitionHandler.CheckToJumpState);
        fallTransitionsList.Add(transitionHandler.CheckToDoubleJumpState);
        fallTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        fallTransitionsList.Add(transitionHandler.CheckToFullStopState);
        fallTransitionsList.Add(transitionHandler.CheckToBurstState);
        fallTransitionsList.Add(transitionHandler.CheckToGrapplePoleAnchoredState);
        fallTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        fallTransitionsList.Add(transitionHandler.CheckToWallRunState);
        fallState = new FallState(this, fallTransitionsList);

        // Double Jump
        List<Func<BasicState>> doubleJumpTransitionsList = new List<Func<BasicState>>();
        doubleJumpTransitionsList.Add(transitionHandler.CheckToFallState);
        doubleJumpTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        doubleJumpTransitionsList.Add(transitionHandler.CheckToFullStopState);
        doubleJumpTransitionsList.Add(transitionHandler.CheckToBurstState);
        doubleJumpTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        doubleJumpTransitionsList.Add(transitionHandler.CheckToWallRunState);
        doubleJumpTransitionsList.Add(transitionHandler.CheckToGrapplePoleAnchoredState);
        doubleJumpTransitionsList.Add(transitionHandler.CheckToWallRunState);
        doubleJumpState = new DoubleJumpState(this, doubleJumpTransitionsList);

        // Full Stop
        List<Func<BasicState>> fullStopTransitionsList = new List<Func<BasicState>>();
        fullStopTransitionsList.Add(transitionHandler.CheckToFallState);
        fullStopState = new FullStopState(this, fullStopTransitionsList);

        // Grapple Pole Anchor
        List<Func<BasicState>> grapplePoleAnchorTransitions = new List<Func<BasicState>>();
        grapplePoleAnchorTransitions.Add(transitionHandler.CheckToFallState);
        grapplePoleAnchorTransitions.Add(transitionHandler.CheckToJumpState);
        grapplePoleAnchorTransitions.Add(transitionHandler.CheckToGrappleGroundPoundState);
        grapplePoleAnchorTransitions.Add(transitionHandler.CheckToGrappleBurstState);
        grapplePoleAnchorTransitions.Add(transitionHandler.CheckToGrappleFullStopState);
        grapplePoleAnchoredState = new GrapplePoleAnchoredState(this, grapplePoleAnchorTransitions);

        // Grapple pole fullstop
        List<Func<BasicState>> grapplePoleFullStopTransitions = new List<Func<BasicState>>();
        grapplePoleFullStopTransitions.Add(transitionHandler.CheckToGrapplePoleAnchoredState);
        grapplePoleFullStopState = new GrappleFullstopState(this, grapplePoleFullStopTransitions);

        // Grapple pole ground pound
        List<Func<BasicState>> grapplePoleGroundPoundTransitions = new List<Func<BasicState>>();
        grapplePoleGroundPoundTransitions.Add(transitionHandler.CheckToGrapplePoleAnchoredState);
        grapplePoleGroundPoundTransitions.Add(transitionHandler.CheckToGrappleFullStopState);
        grapplePoleAnchorTransitions.Add(transitionHandler.CheckToFallState);
        grapplePoleAnchorTransitions.Add(transitionHandler.CheckToJumpState);
        grapplePoleAnchorTransitions.Add(transitionHandler.CheckToGrappleBurstState);
        grapplePoleGroundPoundState = new GrappleGroundPoundState(this, grapplePoleGroundPoundTransitions);

        // Grapple pole burst
        grapplePoleBurstState = new GrappleBurstState(this, null);

        // Ground Pound
        List<Func<BasicState>> groundPoundTransitionsList = new List<Func<BasicState>>();
        groundPoundTransitionsList.Add(transitionHandler.CheckToFallState);
        groundPoundTransitionsList.Add(transitionHandler.CheckToJumpState);
        groundPoundTransitionsList.Add(transitionHandler.CheckToFullStopState);
        groundPoundTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        groundPoundTransitionsList.Add(transitionHandler.CheckToDoubleJumpState);
        groundPoundTransitionsList.Add(transitionHandler.CheckToBurstState);
        groundPoundTransitionsList.Add(transitionHandler.CheckToGrappleGroundPoundState);
        groundPoundState = new GroundPoundState(this, groundPoundTransitionsList);

        // Upward Dash
        List<Func<BasicState>> upwardDashTransitionsList = new List<Func<BasicState>>();
        upwardDashTransitionsList.Add(transitionHandler.CheckToFallState);
        upwardDashTransitionsList.Add(transitionHandler.CheckToFullStopState);
        upwardDashTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        upwardDashTransitionsList.Add(transitionHandler.CheckToDoubleJumpState);
        upwardDashTransitionsList.Add(transitionHandler.CheckToGrapplePoleAnchoredState);
        upwardDashState = new UpwardDashState(this, upwardDashTransitionsList);

        // Wall Run
        List<Func<BasicState>> wallRunTransitionsList = new List<Func<BasicState>>();
        wallRunTransitionsList.Add(transitionHandler.CheckToFallState);
        wallRunTransitionsList.Add(transitionHandler.CheckToIdleState);
        wallRunTransitionsList.Add(transitionHandler.CheckToGrapplePoleAnchoredState);
        wallRunTransitionsList.Add(transitionHandler.CheckToFullStopState);
        wallRunTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        wallRunTransitionsList.Add(transitionHandler.CheckToBurstState);
        wallRunTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        wallRunState = new WallRunState(this, wallRunTransitionsList);

        // Wall Run Wall Running
        List<Func<BasicState>> wallRunSideWallRunningTransitions = new List<Func<BasicState>>();
        wallRunSideWallRunningTransitions.Add(transitionHandler.CheckToWallRunWallClimbingState);
        wallRunSideWallRunningTransitions.Add(transitionHandler.CheckToWallRunWallJumpState);
        wallRunState.GetSubStateArray()[0] = new SideWallRunningState(this, wallRunSideWallRunningTransitions);

        // Burst
        burstState = new BurstState(this, null);
    }

    public override void OnValidate()
    {
        abilities = GetComponent<PlayerAbilities>();
        playerCM = GetComponent<PhysicsManager>().collisionManager;
    }

    private void PlayerSpawned()
    {
        currentState.ExitState();
        currentState = idleState;
        previousState = idleState;
        currentState.EntryState();
    }

    public override void OnEnable()
    {
        currentState = idleState;
        previousState = idleState;
        currentState.EntryState();
        playerCM.CollisionDataCollected += UpdateLoop;

        LevelController.PlayerRespawned += PlayerSpawned;
    }

    public override void OnDisable()
    {
        currentState.ExitState();
        playerCM.CollisionDataCollected -= UpdateLoop;

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
        initialGroundPoundBurstCoolDown.CountDownFixed();
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
        if (transitionHandler.grapplePressed.value && abilities.grapplePoleBehavior.UnlockedAndReady)
        {
            abilities.grapplePoleBehavior.EntryAction();
            grappleFiring = true;
        }
        else if (transitionHandler.grapplePressed.value && abilities.grapplePoleBehavior.Unlocked && abilities.grapplePoleBehavior.Consumed)
        {
            abilities.grapplePoleBehavior.PlayerSoundManager().Play("GrappleFail");
        }
        else if (transitionHandler.grappleReleased.value || currentState == grapplePoleAnchoredState)
        {
            if (!abilities.grapplePoleBehavior.IsAnchored())
            {
                abilities.grapplePoleBehavior.ResetPole();
            }
            grappleFiring = false;
        }
        else if (grappleFiring && !abilities.grapplePoleBehavior.IsAnchored())
        {
            abilities.grapplePoleBehavior.GrappleTransition();
        }
    }
}