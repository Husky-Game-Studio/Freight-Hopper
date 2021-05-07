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
    private bool jumpPressed = false;

    [SerializeField] public Timer jumpHoldingTimer = new Timer(0.5f);
    [SerializeField] public Timer coyoteeTimer = new Timer(0.5f);
    [SerializeField] public Timer jumpBufferTimer = new Timer(0.3f);

    // Player States

    public IdleState idleState;// = new IdleState();
    public FallState fallState;// = new FallState();
    public RunState runState;// = new RunState();

    // If it has a substate use the constructor

    public JumpState jumpState;// = new JumpState();
    public DoubleJumpState doubleJumpState;// = new DoubleJumpState();
    public GroundPoundState groundPoundState;// = new GroundPoundState();
    public BurstState burstState = new BurstState();
    public FullStopState fullStopState;// = new FullStopState();
    public UpwardDashState upwardDashState;// = new UpwardDashState();
    public WallRunState wallRunState;
    public GrapplePoleState grapplePoleState;

    // Input Components
    [HideInInspector] public PlayerAbilities abilities;

    [HideInInspector] public CollisionManagement playerCM;



    void Awake() { 
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
        idleState = new IdleState(idleTransitionsList);

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
        runState = new RunState(runTransitionsList);

        // Jump Transitions
        List<Func<BasicState>> jumpTransitionsList = new List<Func<BasicState>>();
        jumpTransitionsList.Add(pFSMTH.checkToFallState);
        jumpTransitionsList.Add(pFSMTH.checkToGrapplePoleState);
        jumpState = new JumpState(jumpTransitionsList);

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
        fallState = new FallState(fallTransitionsList);

        // Double Jump Transitions
        List<Func<BasicState>> doubleJumpTransitionsList = new List<Func<BasicState>>();
        doubleJumpTransitionsList.Add(pFSMTH.checkToFallState);
        doubleJumpTransitionsList.Add(pFSMTH.checkToGrapplePoleState);
        doubleJumpState = new DoubleJumpState(doubleJumpTransitionsList);

        // Full Stop Transitions
        List<Func<BasicState>> fullStopTransitionsList = new List<Func<BasicState>>();
        fullStopTransitionsList.Add(pFSMTH.checkToFallState);
        fullStopState = new FullStopState(fullStopTransitionsList);

        // Grapple Pole Transitions
        List<Func<BasicState>> grapplePoleTransistionsList = new List<Func<BasicState>>();
        grapplePoleTransistionsList.Add(pFSMTH.checkToFallState);
        grapplePoleTransistionsList.Add(pFSMTH.checkToJumpState);
        grapplePoleState = new GrapplePoleState(this, grapplePoleTransistionsList);

        // Gapple Pole Grapple Fire Transitions
        List<Func<BasicState>> grapplePoleGrappleFireTransitionsList = new List<Func<BasicState>>();
        grapplePoleGrappleFireTransitionsList.Add(pFSMTH.checkToGrapplePoleAnchoredState);
        grapplePoleState.GetSubStateArray()[0] = new GrappleFireState(grapplePoleGrappleFireTransitionsList);

        // Ground Pound Transitions
        List<Func<BasicState>> groundPoundTransitionsList = new List<Func<BasicState>>();
        groundPoundTransitionsList.Add(pFSMTH.checkToFallState);
        groundPoundTransitionsList.Add(pFSMTH.checkToJumpState);
        groundPoundTransitionsList.Add(pFSMTH.checkToDoubleJumpState);
        groundPoundState = new GroundPoundState(groundPoundTransitionsList);

        // Upward Dash Transitions
        List<Func<BasicState>> upwardDashTransitionsList = new List<Func<BasicState>>();
        upwardDashTransitionsList.Add(pFSMTH.checkToFallState);
        upwardDashTransitionsList.Add(pFSMTH.checkToGrapplePoleState);
        upwardDashState = new UpwardDashState(upwardDashTransitionsList);

        // Wall Run Transisitons
        List<Func<BasicState>> wallRunTransitionsList = new List<Func<BasicState>>();
        wallRunTransitionsList.Add(pFSMTH.checkToFallState);
        wallRunTransitionsList.Add(pFSMTH.checkToIdleState);
        wallRunState = new WallRunState(this, wallRunTransitionsList);

        // Wall Run Wall Running Transitions
        List<Func<BasicState>> wallRunSideWallRunningTransitions = new List<Func<BasicState>>();
        wallRunSideWallRunningTransitions.Add(pFSMTH.checkToWallRunWallClimbingState);
        wallRunSideWallRunningTransitions.Add(pFSMTH.checkToWallRunWallJumpState);
        wallRunState.GetSubStateArray()[0] = new SideWallRunningState(wallRunSideWallRunningTransitions);
    }

    public override void OnValidate()
    {
        abilities = GetComponent<PlayerAbilities>();
        playerCM = GetComponent<CollisionManagement>();
    }

    private void PlayerSpawned()
    {
        currentState.UnsubToListeners(this);
        currentState = idleState;
        previousState = idleState;
        currentState.SubToListeners(this);
    }

    public override void OnEnable()
    {
        currentState = idleState;
        previousState = idleState;
        currentState.SubToListeners(this);
        playerCM.CollisionDataCollected += LateFixedUpdate;

        LevelController.PlayerRespawned += PlayerSpawned;
        UserInput.Instance.JumpInput += this.JumpButtonPressed;
    }

    public override void OnDisable()
    {
        currentState.UnsubToListeners(this);
        playerCM.CollisionDataCollected -= LateFixedUpdate;

        LevelController.PlayerRespawned -= PlayerSpawned;
        UserInput.Instance.JumpInput -= this.JumpButtonPressed;
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