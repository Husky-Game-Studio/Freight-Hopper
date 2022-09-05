using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private MovementBehavior movementBehavior;
    [SerializeField] private JumpBehavior jumpBehavior;
    [SerializeField] private GroundPoundBehavior groundPoundBehavior;
    [SerializeField] private WallRunBehavior wallBehavior;
    
    private CollisionManagement collisionManagement;
    private Friction friction;

    private bool jumpedPreviously;

    private void Awake()
    {
        collisionManagement = Player.Instance.modules.collisionManagement;
        friction = Player.Instance.modules.friction;
        wallBehavior.Initialize();
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

        UserInput.Instance.GroundPoundInput -= GroundPoundStartLogic;
        UserInput.Instance.GroundPoundCanceled -= groundPoundBehavior.ExitAction;

        UserInput.Instance.JumpInput -= JumpsStartLogic;
        UserInput.Instance.JumpInputCanceled -= JumpsCanceledLogic;
    }

    private void GroundPoundStartLogic()
    {
        if(!InGameStates.Instance.StateIs(InGameStates.States.Playing)) 
        {
            return;
        }
        if(groundPoundBehavior.FlatSurface) 
        {
            return;
        }
        groundPoundBehavior.EntryAction();
    }

    private void JumpsStartLogic()
    {
        if(!InGameStates.Instance.StateIs(InGameStates.States.Playing)) 
        {
            return;
        }
        if (wallBehavior.RunActive || wallBehavior.ClimbActive) {
            // Wall Jump Logic
            if (wallBehavior.JumpActive)
            {
                return;
            }
            jumpedPreviously = true;
            wallBehavior.JumpInitial();
            return;
        }

        if (jumpBehavior.Consumed)
        {
            jumpBehavior.PlayerSoundManager().Play("JumpFail");
            return;
        }
        bool canJump = jumpBehavior.coyoteeTimer.TimerActive() || collisionManagement.IsGrounded.old;
        if(!canJump) 
        {
            return;
        }
        
        if (jumpBehavior.Active)
        {
            jumpBehavior.coyoteeTimer.DeactivateTimer();
            return;
        }
        jumpedPreviously = true;
        jumpBehavior.EntryAction();
    }

    private void JumpsCanceledLogic()
    {
        if (wallBehavior.JumpActive)
        {
            wallBehavior.JumpExit();
            return;
        }

        if (jumpBehavior.Active)
        {
            jumpBehavior.ExitAction();
        }
    }
    
    private void UpdateLoop()
    {
        movementBehavior.UpdateStatus();
        // WALL RUN ENTRY LOGIC ========================================================
        if(!wallBehavior.RunActive && !wallBehavior.JumpActive) {
            if (!wallBehavior.RunActive && !wallBehavior.inAirCooldown.TimerActive())
            {
                if (wallBehavior.ShouldLeftRun){
                    wallBehavior.RunInitial();
                }
                if (wallBehavior.ShouldRightRun)
                {
                    wallBehavior.RunInitial();
                }
            }
        }

        // Wall Climb Entry Logic ========================================================
        if (wallBehavior.ShouldWallClimb && UserInput.Instance.Move().z >= 0.6f &&
            !wallBehavior.ClimbActive)
        {
            wallBehavior.InitialWallClimb();
        }
        
        // Moving ========================================================
        movementBehavior.Action();
        
        // APPLY ABILITIES ========================================================
        if(UserInput.Instance.JumpHeld && !jumpedPreviously)
        {
            JumpsStartLogic();
        }
        if(groundPoundBehavior.Active)
        {
            groundPoundBehavior.Action();
        }
        if (jumpBehavior.Active && !wallBehavior.JumpActive)
        {
            jumpBehavior.jumpHoldingTimer.CountDown(Time.fixedDeltaTime);
            jumpBehavior.Action();
        }
        if (wallBehavior.JumpActive)
        {
            wallBehavior.WallJumpContinous();
        }
        else if (wallBehavior.RunActive)
        {
            if (wallBehavior.ShouldRightRun)
            {
                wallBehavior.RightRun();
            }
            if (wallBehavior.ShouldLeftRun)
            {
                wallBehavior.LeftRun();
            }
        }
        else if(wallBehavior.ClimbActive)
        {
            wallBehavior.Climb();
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
        if (!wallBehavior.jumpHoldingTimer.TimerActive() && wallBehavior.JumpActive)
        {
            wallBehavior.JumpExit();
        }
        if (!jumpBehavior.jumpHoldingTimer.TimerActive() && jumpBehavior.Active){
            jumpBehavior.ExitAction();
        }

        // Wall Run Exit Logic ========================================================
        if(wallBehavior.RunActive){
            if (!wallBehavior.ShouldLeftRun && !wallBehavior.ShouldRightRun)
            {
                if (!wallBehavior.coyoteTimer.TimerActive())
                {
                    wallBehavior.coyoteTimer.ResetTimer();
                    wallBehavior.RunExit();
                }
            }
        }
        
        // Wall Climb Exit Logic ========================================================
        if (wallBehavior.ClimbActive) {
            if(!wallBehavior.ShouldWallClimb || UserInput.Instance.Move().z < 0.6f) {
                wallBehavior.WallClimbExit();
            }
        }

        // Wall Jump Exit Logic ========================================================
        if (wallBehavior.JumpActive)
        {
            if (!wallBehavior.jumpHoldingTimer.TimerActive() || !UserInput.Instance.JumpHeld)
            {
                wallBehavior.JumpExit();
            }
        }
        
        // Reset Values ========================================================
        jumpedPreviously = false;
    }
}