using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] MovementBehavior movementBehavior;
    [SerializeField] JumpBehavior jumpBehavior;
    [SerializeField] GroundPoundBehavior groundPoundBehavior;
    [SerializeField] WallRunBehavior wallRunBehavior;

    private FirstPersonCamera cameraController;
    private CollisionManagement collisionManagement;
    private Friction friction;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<FirstPersonCamera>();
        collisionManagement = Player.Instance.modules.collisionManagement;
        friction = Player.Instance.modules.friction;
        wallRunBehavior.Initialize();
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
    void GroundPoundStartLogic()
    {
        if(groundPoundBehavior.FlatSurface) 
        {
            return;
        }
        groundPoundBehavior.EntryAction();
    }
    void GroundPoundExitLogic() 
    {
        bool shouldExit = groundPoundBehavior.Active && groundPoundBehavior.FlatSurface;
        if (!shouldExit) 
        {
            return;
        }
        groundPoundBehavior.ExitAction();
    }

    void JumpsStartLogic()
    {
        if(wallRunBehavior.WallRunActive) {
            WallJumpLogic();
            return;
        }

        jumpBehavior.jumpBufferTimer.ResetTimer();
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
        jumpBehavior.EntryAction();
    }
    void WallJumpLogic()
    {
        wallRunBehavior.WallJumpInitial();
    }

    void JumpsCanceledLogic()
    {
        if (wallRunBehavior.WallJumpActive)
        {
            wallRunBehavior.ExitAction();
            return;
        }

        if (jumpBehavior.Active)
        {
            jumpBehavior.ExitAction();
        }
    }
    void JumpsExitLogic()
    {
        if(!jumpBehavior.jumpHoldingTimer.TimerActive()){
            jumpBehavior.ExitAction();
        }
        if (!wallRunBehavior.jumpHoldingTimer.TimerActive() && wallRunBehavior.WallJumpActive)
        {
            wallRunBehavior.ExitAction();
        }
    }
    void MovingLogic()
    {
        bool cantMove = false;
        if(cantMove)
        {
            return;
        }

        movementBehavior.Action();
    }
    void ResetFrictionOnInactivity()
    {
        if (UserInput.Instance.Move().IsZero() && !groundPoundBehavior.Active)
        {
            friction.ResetFrictionReduction();
        }
    }
    void ApplyAbilities()
    {
        if(groundPoundBehavior.Active)
        {
            groundPoundBehavior.Action();
        }
        if (jumpBehavior.Active)
        {
            jumpBehavior.jumpHoldingTimer.CountDown(Time.fixedDeltaTime);
            if (!collisionManagement.IsGrounded.current)
            {
                jumpBehavior.coyoteeTimer.DeactivateTimer();
            }
            jumpBehavior.Action();
        }
        if(wallRunBehavior.WallRunActive)
        {
            IList<bool> status = wallRunBehavior.WallStatus;
            if (status[2])
            {
                wallRunBehavior.RightWallRun();
            }
            if (status[0])
            {
                wallRunBehavior.LeftWallRun();
            }
        }
        if(wallRunBehavior.WallClimbActive)
        {
            wallRunBehavior.WallClimb();
        }
    }

    private void WallRunEntryLogic()
    {
        if (!wallRunBehavior.WallRunActive && !wallRunBehavior.inAirCooldown.TimerActive())
        {
            IList<bool> walls = wallRunBehavior.WallStatus;
            if (walls[0] || walls[1] || walls[2])
            {
                wallRunBehavior.EntryAction();
            }
        }
    }
    private void WallClimbEntryLogic()
    {
        IList<bool> status = wallRunBehavior.WallStatus;
        if (status[1] && !status[0] && !status[2] && UserInput.Instance.Move().z == 1){
            wallRunBehavior.InitialWallClimb();
        }
    }
    private void WallRunExitLogic(){
        IList<bool> status = wallRunBehavior.WallStatus;
        // Fall from wall climb
        if ((!status[0] && !status[1] && !status[2] &&
            wallRunBehavior.WallRunActive) || UserInput.Instance.Move().z != 1)
        {
            wallRunBehavior.coyoteTimer.CountDown(Time.fixedDeltaTime);
            if (!wallRunBehavior.coyoteTimer.TimerActive())
            {
                wallRunBehavior.coyoteTimer.ResetTimer();
                wallRunBehavior.ExitAction();
            }
        }

        if (wallRunBehavior.WallClimbActive && !status[1])
        {
            wallRunBehavior.ExitAction();
        }
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
        EndLoop();
    }
    
    private void EndLoop()
    {
        ResetInputs();
    }
    private void ResetInputs()
    {

    }
}
