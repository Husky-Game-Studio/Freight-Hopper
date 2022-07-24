using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private MovementBehavior movementBehavior;
    [SerializeField] private JumpBehavior jumpBehavior;
    [SerializeField] private GroundPoundBehavior groundPoundBehavior;
    [SerializeField] private WallRunBehavior wallBehavior;

    private FirstPersonCamera cameraController;
    private CollisionManagement collisionManagement;
    private Friction friction;

    private bool jumpedPreviously;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<FirstPersonCamera>();
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
        if(Time.timeScale == 0) 
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
        if (Time.timeScale == 0)
        {
            return;
        }
        if (wallBehavior.RunActive || wallBehavior.ClimbActive) {
            WallJumpLogic();
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

    private void WallJumpLogic()
    {
        if (wallBehavior.JumpActive)
        {
            return;
        }
        jumpedPreviously = true;
        wallBehavior.JumpInitial();
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

    private void JumpsExitLogic()
    {
        if (!wallBehavior.jumpHoldingTimer.TimerActive() && wallBehavior.JumpActive)
        {
            wallBehavior.JumpExit();
            return;
        }
        if (!jumpBehavior.jumpHoldingTimer.TimerActive()){
            jumpBehavior.ExitAction();
            
        }
        
    }

    private void MovingLogic()
    {
        movementBehavior.Action();
    }

    private void ResetFrictionOnInactivity()
    {
        if (UserInput.Instance.Move().IsZero() && !groundPoundBehavior.Active)
        {
            friction.ResetFrictionReduction();
        }
    }

    private void ApplyAbilities()
    {
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
        
    }

    private void WallRunEntryLogic()
    {
        if(wallBehavior.RunActive || wallBehavior.JumpActive) {
            return;
        }
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
    private void WallClimbEntryLogic()
    {
        if (wallBehavior.ShouldWallClimb && UserInput.Instance.Move().z >= 0.6f &&
            !wallBehavior.ClimbActive)
        {
            wallBehavior.InitialWallClimb();
        }
    }
    private void WallRunExitLogic(){
        if(!wallBehavior.RunActive){
            return;
        }
        if (!wallBehavior.ShouldLeftRun && !wallBehavior.ShouldRightRun)
        {
            if (!wallBehavior.coyoteTimer.TimerActive())
            {
                wallBehavior.coyoteTimer.ResetTimer();
                wallBehavior.RunExit();
            }
        }
    }

    private void WallClimbExitLogic(){
        if (!wallBehavior.ClimbActive) {
            return;
        }

        if(!wallBehavior.ShouldWallClimb || UserInput.Instance.Move().z < 0.6f) {
            wallBehavior.WallClimbExit();
        }
    }

    private void WallJumpExitLogic(){
        if (!wallBehavior.JumpActive)
        {
            return;
        }

        if (!wallBehavior.jumpHoldingTimer.TimerActive() || !UserInput.Instance.JumpHeld)
        {
            wallBehavior.JumpExit();
        }
    }

    private void GroundPoundExitLogic()
    {
        bool shouldExit = groundPoundBehavior.Active && groundPoundBehavior.FlatSurface;
        if (!shouldExit)
        {
            return;
        }
        groundPoundBehavior.ExitAction();
    }
    private void UpdateLoop()
    {
        movementBehavior.UpdateStatus();
        WallRunEntryLogic();
        WallClimbEntryLogic();
        MovingLogic();
        ApplyAbilities(); 
        
        ResetFrictionOnInactivity();

        GroundPoundExitLogic();
        JumpsExitLogic();

        WallRunExitLogic();
        WallClimbExitLogic();
        WallJumpExitLogic();
        ResetValues();
    }
    
    private void ResetValues()
    {
        jumpedPreviously = false;
    }
}
