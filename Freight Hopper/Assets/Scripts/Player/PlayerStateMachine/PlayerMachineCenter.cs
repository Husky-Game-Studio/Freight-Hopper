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
    //public GrappleBurstState grapplePoleBurstState;

    // State independent fields
    [SerializeField, ReadOnly] private bool grappleFiring;

    [SerializeField] private GameObject defaultCrosshair;
    [SerializeField] private GameObject grappleCrosshair;
    public Timer initialGroundPoundBurstCoolDown;

    // Input Components
    [HideInInspector] public PlayerAbilities abilities;

    [HideInInspector] public PhysicsManager physicsManager;
    [HideInInspector] public CollisionManagement collisionManagement;

    private void Awake()
    {
        transitionHandler = new PlayerStatesTransitions(this);

        // Default
        List<Func<BasicState>> defaultTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckToMoveState,
            transitionHandler.CheckToJumpState,
            transitionHandler.CheckToDoubleJumpState,
            transitionHandler.CheckToDefaultState,
            transitionHandler.CheckToGroundPoundState,
            transitionHandler.CheckToFullStopState,
            transitionHandler.CheckToBurstState,
            transitionHandler.CheckToUpwardDashState,
            transitionHandler.CheckToWallRunState,
            transitionHandler.CheckToGrapplePoleAnchoredState
        };
        defaultState = new DefaultState(this, defaultTransitionsList);

        // Move
        List<Func<BasicState>> moveTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckToDefaultState,
            transitionHandler.CheckToJumpState,
            transitionHandler.CheckToDoubleJumpState,
            transitionHandler.CheckToGroundPoundState,
            transitionHandler.CheckToFullStopState,
            transitionHandler.CheckToBurstState,
            transitionHandler.CheckToWallRunState,
            transitionHandler.CheckToUpwardDashState,
            transitionHandler.CheckToGrapplePoleAnchoredState
        };
        moveState = new MoveState(this, moveTransitionsList);

        // Jump
        List<Func<BasicState>> jumpTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckToDefaultState,
            transitionHandler.CheckToGroundPoundState,
            transitionHandler.CheckToFullStopState,
            transitionHandler.CheckToBurstState,
            transitionHandler.CheckToUpwardDashState,
            transitionHandler.CheckToWallRunState,
            transitionHandler.CheckToGrapplePoleAnchoredState
        };
        jumpState = new JumpState(this, jumpTransitionsList);

        // Double Jump
        List<Func<BasicState>> doubleJumpTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckToDefaultState,
            transitionHandler.CheckToGroundPoundState,
            transitionHandler.CheckToFullStopState,
            transitionHandler.CheckToBurstState,
            transitionHandler.CheckToUpwardDashState,
            transitionHandler.CheckToWallRunState,
            transitionHandler.CheckToGrapplePoleAnchoredState,
            transitionHandler.CheckToWallRunState
        };
        doubleJumpState = new DoubleJumpState(this, doubleJumpTransitionsList);

        // Full Stop
        List<Func<BasicState>> fullStopTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckToDefaultState
        };
        fullStopState = new FullStopState(this, fullStopTransitionsList);

        // Grapple Pole Anchor
        List<Func<BasicState>> grapplePoleAnchorTransitions = new List<Func<BasicState>>
        {
            transitionHandler.CheckToDefaultState,
            transitionHandler.CheckToJumpState,
            transitionHandler.CheckToGrappleGroundPoundState,
            transitionHandler.CheckToGrappleFullStopState
        };
        grapplePoleAnchoredState = new GrapplePoleAnchoredState(this, grapplePoleAnchorTransitions);

        // Grapple pole fullstop
        List<Func<BasicState>> grapplePoleFullStopTransitions = new List<Func<BasicState>>
        {
            transitionHandler.CheckToGrapplePoleAnchoredState
        };
        grapplePoleFullStopState = new GrappleFullstopState(this, grapplePoleFullStopTransitions);

        // Grapple pole ground pound
        List<Func<BasicState>> grapplePoleGroundPoundTransitions = new List<Func<BasicState>>
        {
            transitionHandler.CheckToGrapplePoleAnchoredState,
            transitionHandler.CheckToGrappleFullStopState,
            transitionHandler.CheckToDefaultState,
            transitionHandler.CheckToJumpState,
        };

        grapplePoleGroundPoundState = new GrappleGroundPoundState(this, grapplePoleGroundPoundTransitions);

        // Grapple pole burst
        //grapplePoleBurstState = new GrappleBurstState(this, null);

        // Ground Pound
        List<Func<BasicState>> groundPoundTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckToDefaultState,
            transitionHandler.CheckToJumpState,
            transitionHandler.CheckToFullStopState,
            transitionHandler.CheckToUpwardDashState,
            transitionHandler.CheckToDoubleJumpState,
            transitionHandler.CheckToBurstState,
            transitionHandler.CheckToGrappleGroundPoundState
        };
        groundPoundState = new GroundPoundState(this, groundPoundTransitionsList);

        // Upward Dash
        List<Func<BasicState>> upwardDashTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckToDefaultState,
            transitionHandler.CheckToFullStopState,
            transitionHandler.CheckToGroundPoundState,
            transitionHandler.CheckToDoubleJumpState,
            transitionHandler.CheckToGrapplePoleAnchoredState
        };
        upwardDashState = new UpwardDashState(this, upwardDashTransitionsList);

        // Wall Run
        List<Func<BasicState>> wallRunTransitionsList = new List<Func<BasicState>>
        {
            transitionHandler.CheckToDefaultState,
            transitionHandler.CheckToGrapplePoleAnchoredState,
            transitionHandler.CheckToFullStopState,
            transitionHandler.CheckToGroundPoundState,
            transitionHandler.CheckToBurstState,
            transitionHandler.CheckToUpwardDashState
        };
        wallRunState = new WallRunState(this, wallRunTransitionsList);

        // Wall Run Wall Running
        List<Func<BasicState>> wallRunSideWallRunningTransitions = new List<Func<BasicState>>
        {
            transitionHandler.CheckToWallRunWallClimbingState,
            transitionHandler.CheckToWallRunWallJumpState
        };
        wallRunState.GetSubStateArray()[0] = new SideWallRunningState(this, wallRunSideWallRunningTransitions);

        // Burst
        burstState = new BurstState(this, null);
    }

    public void OnEnable()
    {
        abilities = GetComponent<PlayerAbilities>();
        physicsManager = GetComponent<PhysicsManager>();
        collisionManagement = physicsManager.collisionManager;
        RestartFSM();
        collisionManagement.CollisionDataCollected += UpdateLoop;
        collisionManagement.Landed += abilities.Recharge;
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
        abilities.movementBehavior.UpdateMovement();
        if (collisionManagement.IsGrounded.current)
        {
            abilities.jumpBehavior.coyoteeTimer.ResetTimer();
            abilities.wallRunBehavior.inAirCooldown.ResetTimer();
        }
        else
        {
            abilities.jumpBehavior.coyoteeTimer.CountDownFixed();
            abilities.wallRunBehavior.inAirCooldown.CountDownFixed();
        }

        if (UserInput.Instance.Move().IsZero() && (currentState != groundPoundState || currentState != grapplePoleGroundPoundState))
        {
            physicsManager.friction.ResetFrictionReduction();
        }

        if (collisionManagement.IsGrounded.current && !collisionManagement.IsGrounded.old)
        {
            initialGroundPoundBurstCoolDown.DeactivateTimer();
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
        else if (transitionHandler.jumpPressed.value || transitionHandler.burstPressed.value
            || currentState == grapplePoleAnchoredState)
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