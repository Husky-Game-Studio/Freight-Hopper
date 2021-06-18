using System.Collections.Generic;
using UnityEngine;
using System;

// State Machine help from these url:
// https://www.youtube.com/watch?v=nnrOhb5UdRc

public class PlayerMachineCenter : FiniteStateMachineCenter
{
    // Player State Machine Transition Handler
    private PlayerStatesTransitions transitionHandler;

    // Player States
    public MoveState moveState;
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

    [SerializeField] private GameObject defaultCrosshair;
    [SerializeField] private GameObject grappleCrosshair;
    [SerializeField] public Timer initialGroundPoundBurstCoolDown;

    // Input Components
    [HideInInspector] public PlayerAbilities abilities;

    [HideInInspector] public PhysicsManager playerPM;
    [HideInInspector] public CollisionManagement playerCM;

    private void Awake()
    {
        transitionHandler = new PlayerStatesTransitions(this);

        // Default
        List<Func<BasicState>> defaultTransitionsList = new List<Func<BasicState>>();
        defaultTransitionsList.Add(transitionHandler.CheckToMoveState);
        defaultTransitionsList.Add(transitionHandler.CheckToJumpState);
        defaultTransitionsList.Add(transitionHandler.CheckToDoubleJumpState);
        defaultTransitionsList.Add(transitionHandler.CheckToDefaultState);
        defaultTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        defaultTransitionsList.Add(transitionHandler.CheckToFullStopState);
        defaultTransitionsList.Add(transitionHandler.CheckToBurstState);
        defaultTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        defaultTransitionsList.Add(transitionHandler.CheckToWallRunState);
        defaultTransitionsList.Add(transitionHandler.CheckToGrapplePoleAnchoredState);
        defaultState = new DefaultState(this, defaultTransitionsList);

        // Move
        List<Func<BasicState>> moveTransitionsList = new List<Func<BasicState>>();
        moveTransitionsList.Add(transitionHandler.CheckToDefaultState);
        moveTransitionsList.Add(transitionHandler.CheckToJumpState);
        moveTransitionsList.Add(transitionHandler.CheckToDoubleJumpState);
        moveTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        moveTransitionsList.Add(transitionHandler.CheckToFullStopState);
        moveTransitionsList.Add(transitionHandler.CheckToBurstState);
        moveTransitionsList.Add(transitionHandler.CheckToWallRunState);
        moveTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        moveTransitionsList.Add(transitionHandler.CheckToGrapplePoleAnchoredState);
        moveState = new MoveState(this, moveTransitionsList);

        // Jump
        List<Func<BasicState>> jumpTransitionsList = new List<Func<BasicState>>();
        jumpTransitionsList.Add(transitionHandler.CheckToDefaultState);
        jumpTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        jumpTransitionsList.Add(transitionHandler.CheckToFullStopState);
        jumpTransitionsList.Add(transitionHandler.CheckToBurstState);
        jumpTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        jumpTransitionsList.Add(transitionHandler.CheckToWallRunState);
        jumpTransitionsList.Add(transitionHandler.CheckToGrapplePoleAnchoredState);
        jumpState = new JumpState(this, jumpTransitionsList);

        // Double Jump
        List<Func<BasicState>> doubleJumpTransitionsList = new List<Func<BasicState>>();
        doubleJumpTransitionsList.Add(transitionHandler.CheckToDefaultState);
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
        fullStopTransitionsList.Add(transitionHandler.CheckToDefaultState);
        fullStopState = new FullStopState(this, fullStopTransitionsList);

        // Grapple Pole Anchor
        List<Func<BasicState>> grapplePoleAnchorTransitions = new List<Func<BasicState>>();
        grapplePoleAnchorTransitions.Add(transitionHandler.CheckToDefaultState);
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
        grapplePoleAnchorTransitions.Add(transitionHandler.CheckToDefaultState);
        grapplePoleAnchorTransitions.Add(transitionHandler.CheckToJumpState);
        grapplePoleAnchorTransitions.Add(transitionHandler.CheckToGrappleBurstState);
        grapplePoleGroundPoundState = new GrappleGroundPoundState(this, grapplePoleGroundPoundTransitions);

        // Grapple pole burst
        grapplePoleBurstState = new GrappleBurstState(this, null);

        // Ground Pound
        List<Func<BasicState>> groundPoundTransitionsList = new List<Func<BasicState>>();
        groundPoundTransitionsList.Add(transitionHandler.CheckToDefaultState);
        groundPoundTransitionsList.Add(transitionHandler.CheckToJumpState);
        groundPoundTransitionsList.Add(transitionHandler.CheckToFullStopState);
        groundPoundTransitionsList.Add(transitionHandler.CheckToUpwardDashState);
        groundPoundTransitionsList.Add(transitionHandler.CheckToDoubleJumpState);
        groundPoundTransitionsList.Add(transitionHandler.CheckToBurstState);
        groundPoundTransitionsList.Add(transitionHandler.CheckToGrappleGroundPoundState);
        groundPoundState = new GroundPoundState(this, groundPoundTransitionsList);

        // Upward Dash
        List<Func<BasicState>> upwardDashTransitionsList = new List<Func<BasicState>>();
        upwardDashTransitionsList.Add(transitionHandler.CheckToDefaultState);
        upwardDashTransitionsList.Add(transitionHandler.CheckToFullStopState);
        upwardDashTransitionsList.Add(transitionHandler.CheckToGroundPoundState);
        upwardDashTransitionsList.Add(transitionHandler.CheckToDoubleJumpState);
        upwardDashTransitionsList.Add(transitionHandler.CheckToGrapplePoleAnchoredState);
        upwardDashState = new UpwardDashState(this, upwardDashTransitionsList);

        // Wall Run
        List<Func<BasicState>> wallRunTransitionsList = new List<Func<BasicState>>();
        wallRunTransitionsList.Add(transitionHandler.CheckToDefaultState);
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

    public void OnValidate()
    {
        abilities = GetComponent<PlayerAbilities>();
        playerPM = GetComponent<PhysicsManager>();
        playerCM = playerPM.collisionManager;
    }

    public void OnEnable()
    {
        RestartFSM();
        playerCM.CollisionDataCollected += UpdateLoop;
        playerCM.Landed += abilities.Recharge;
        LevelController.PlayerRespawned += RestartFSM;
    }

    public void OnDisable()
    {
        currentState.ExitState();
        playerCM.CollisionDataCollected -= UpdateLoop;

        LevelController.PlayerRespawned -= RestartFSM;
    }

    protected override void EndLoop()
    {
        transitionHandler.ResetInputs();
    }

    // perform anything that is independent of being in any one single state
    public override void PerformStateIndependentBehaviors()
    {
        JumpBuffer();

        if (playerCM.IsGrounded.current)
        {
            abilities.jumpBehavior.coyoteeTimer.ResetTimer();
        }
        else
        {
            abilities.jumpBehavior.coyoteeTimer.CountDown();
        }

        GrappleFiring();
        initialGroundPoundBurstCoolDown.CountDownFixed();
        if (abilities.grapplePoleBehavior.UnlockedAndReady)
        {
            SetCrosshair(abilities.grapplePoleBehavior.CanReachSurface());
        }
        else
        {
            SetCrosshair(false);
        }
    }

    private void SetCrosshair(bool status)
    {
        defaultCrosshair.SetActive(!status);
        grappleCrosshair.SetActive(status);
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