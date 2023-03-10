using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private MovementBehavior movementBehavior;
    [SerializeField] private JumpBehavior jumpBehavior;
    [SerializeField] private GroundPoundBehavior groundPoundBehavior;
    [FormerlySerializedAs("wallBehavior")][SerializeField] private WallRunBehavior wallBehaviors;

    // You change major version, Gabe, when you want to FUCK everyone over in the leaderboards
    // CHOOSE WISELY
    // Minor when you just want to keep track of history I guess in git? Kinda don't need it
    public const int MajorVersion = 1;
    public const int MinorVersion = 0;

    private CollisionManagement collisionManagement;
    private Friction friction;

    private bool jumpedThisFrame;
    private bool poundedThisFrame;

    private void Awake()
    {
        collisionManagement = Player.Instance.modules.collisionManagement;
        friction = Player.Instance.modules.friction;
        wallBehaviors.Initialize();
        movementBehavior.Initialize();
        jumpBehavior.Initialize();
        groundPoundBehavior.Initialize();
    }
    private void OnEnable()
    {
        collisionManagement.CollisionDataCollected += UpdateLoop;
        collisionManagement.Landed += jumpBehavior.Recharge;
        collisionManagement.Landed += groundPoundBehavior.Recharge;

        UserInput.Instance.GroundPoundInput += GroundPoundStartLogic;
        UserInput.Instance.GroundPoundCanceled += groundPoundBehavior.ExitAction;

        UserInput.Instance.JumpInput += JumpsStartLogic;
        UserInput.Instance.JumpInputCanceled += JumpsCanceledLogic;
    }
    private void OnDisable()
    {
        collisionManagement.CollisionDataCollected -= UpdateLoop;
        collisionManagement.Landed -= jumpBehavior.Recharge;
        collisionManagement.Landed -= groundPoundBehavior.Recharge;
    }

    private void GroundPoundStartLogic()
    {
        if (!InGameStates.Instance.StateIs(InGameStates.States.Playing) || poundedThisFrame)
        {
            return;
        }
        if (groundPoundBehavior.FlatSurface)
        {
            return;
        }
        poundedThisFrame = true;
        groundPoundBehavior.EntryAction();
    }

    private void JumpsStartLogic()
    {
        if (!InGameStates.Instance.StateIs(InGameStates.States.Playing) || jumpedThisFrame)
        {
            return;
        }
        if (wallBehaviors.RunActive || 
            wallBehaviors.ClimbActive)
        {
            // Wall Jump Logic
            if (!wallBehaviors.JumpActive)
            {
                jumpedThisFrame = true;
                wallBehaviors.JumpInitial();
            }
            return;
        }

        if (jumpBehavior.Consumed)
        {
            SoundManager sm = jumpBehavior.PlayerSoundManager();
            sm.Play("JumpFail");
            return;
        }
        bool canJump = jumpBehavior.coyoteeTimer.TimerActive() || collisionManagement.IsGrounded.old;
        if (!canJump)
        {
            return;
        }

        if (jumpBehavior.Active)
        {
            jumpBehavior.coyoteeTimer.DeactivateTimer();
            return;
        }
        jumpedThisFrame = true;
        jumpBehavior.EntryAction();
    }

    private void JumpsCanceledLogic()
    {
        if (wallBehaviors.JumpActive)
        {
            wallBehaviors.JumpExit();
        }
        if (jumpBehavior.Active)
        {
            jumpBehavior.ExitAction();
        }
    }

    private void UpdateLoop()
    {
        movementBehavior.UpdateStatus();
        // Wall run entry logic ========================================================
        if (!wallBehaviors.RunActive && 
            !wallBehaviors.JumpActive && 
            !wallBehaviors.inAirCooldown.TimerActive())
        {
            if (wallBehaviors.RightObstructed || 
                wallBehaviors.LeftObstructed)
            {
                wallBehaviors.RunInitial();
            }
        }

        // Wall Climb Entry Logic ========================================================
        if (!wallBehaviors.ClimbActive &&
            wallBehaviors.FrontObstructed && 
            UserInput.Instance.Move().z >= 0.6f)
        {
            wallBehaviors.InitialWallClimb();
        }

        // Moving ========================================================
        movementBehavior.Action();

        // Jump Holding ========================================================
        if (UserInput.Instance.JumpHeld && !jumpedThisFrame)
        {
            JumpsStartLogic();
        }

        // Applying Abilities Constantly ========================================================
        // Applying Ground Pound
        if (groundPoundBehavior.Active)
        {
            groundPoundBehavior.Action();
        }
        // Applying jumping mid-air when holding
        if (jumpBehavior.Active)
        {
            jumpBehavior.Action();
        }
        // Applying wall jumping mid-air when holding
        else if (wallBehaviors.JumpActive)
        {
            wallBehaviors.WallJumpContinous();
        }
        // Wall running applying
        else if (wallBehaviors.RunActive)
        {
            if (wallBehaviors.RightObstructed)
            {
                wallBehaviors.RightRun();
            }
            else if (wallBehaviors.LeftObstructed)
            {
                wallBehaviors.LeftRun();
            }
        }
        // Wall climbing applying
        else if (wallBehaviors.ClimbActive)
        {
            wallBehaviors.Climb();
        }

        // Reset Friction On Inactivity ========================================================
        if (UserInput.Instance.Move().IsZero() && !groundPoundBehavior.Active)
        {
            friction.ResetFrictionReduction();
        }

        // Ground Pound Exit Logic ========================================================
        if (groundPoundBehavior.Active && groundPoundBehavior.FlatSurface)
        {
            groundPoundBehavior.ExitAction();
        }

        // Jump Exit Logic ========================================================
        if (wallBehaviors.JumpActive && !wallBehaviors.jumpHoldingTimer.TimerActive())
        {
            wallBehaviors.JumpExit();
        }
        if (jumpBehavior.Active && !jumpBehavior.jumpHoldingTimer.TimerActive())
        {
            jumpBehavior.ExitAction();
        }

        // Wall Run Exit Logic ========================================================
        if (wallBehaviors.RunActive &&
            !wallBehaviors.LeftObstructed &&
            !wallBehaviors.RightObstructed && 
            !wallBehaviors.coyoteTimer.TimerActive())
        {
            wallBehaviors.coyoteTimer.ResetTimer();
            wallBehaviors.RunExit();
        }

        // Wall Climb Exit Logic ========================================================
        if (wallBehaviors.ClimbActive && 
            (!wallBehaviors.FrontObstructed || UserInput.Instance.Move().z < 0.6))
        {
            wallBehaviors.WallClimbExit();
        }

        // Wall Jump Exit Logic ========================================================
        if (wallBehaviors.JumpActive &&
            (!wallBehaviors.jumpHoldingTimer.TimerActive() || !UserInput.Instance.JumpHeld))
        {
            wallBehaviors.JumpExit();
        }

        // Reset Values ========================================================
        jumpedThisFrame = false;
        poundedThisFrame = false;
    }
}