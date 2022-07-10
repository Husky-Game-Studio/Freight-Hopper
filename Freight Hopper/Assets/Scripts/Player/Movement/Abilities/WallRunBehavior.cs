using UnityEngine;

public partial class WallRunBehavior : AbilityBehavior
{
    [Header("Wall detection")]
    [SerializeField] private float wallCheckDistance = 2;
    [SerializeField] private float detectionLayerSpacing = 0.75f;
    [SerializeField] private float wallrunCameraTilt = 5;
    [SerializeField] private LayerMask validWalls;
    [SerializeField] private float forwardDetectionTiltAngle = 45;
    [SerializeField] private float backwardDetectionTiltAngle = 30;

    [Space, Header("Wall Running")]
    [SerializeField] private float upwardsForce = 10;
    [SerializeField] private float rightForce = 5;

    [Space, Header("Wall Climbing")]
    [SerializeField] private float initialClimbForce = 5;
    [SerializeField] private float climbForce = 10;

    [Space, Header("Wall Jump")]
    [SerializeField] private float jumpInitialPush = 10;
    [SerializeField] private float againstWallPush = 1;
    [SerializeField] private float jumpContinousUpForce = 10;
    [SerializeField] private float jumpContinousPush = 10;

    [Space, Header("Timers")]
    public Timer jumpHoldingTimer = new Timer(0.5f);
    public Timer inAirCooldown = new Timer(0.1f);
    public Timer coyoteTimer = new Timer(0.75f);
    public Timer entryEffectsCooldown = new Timer(0.1f);
    public Timer exitEffectsCooldown = new Timer(0.1f);

    private Vector3 jumpDirection;
    private Vector3 wallJumpWallDirection;

    [SerializeField, ReadOnly] private FirstPersonCamera cameraController;
    [SerializeField, ReadOnly] private CollisionManagement collisionManager;
    [SerializeField, ReadOnly] private JumpBehavior jumpBehavior;
    [SerializeField, ReadOnly] private RigidbodyLinker rigidbodyLinker;

    // Left, Front, Right
    [SerializeField, ReadOnly] private bool[] wallStatus = new bool[3];
    [SerializeField, ReadOnly] private Vector3[] wallNormals = new Vector3[3];

    //public IList<bool> WallStatus => Array.AsReadOnly(wallStatus);

    public bool ShouldLeftRun => wallStatus[0] && !wallStatus[1];
    public bool ShouldRightRun => wallStatus[2] && !wallStatus[1];
    public bool ShouldWallRun => (wallStatus[0] || wallStatus[2]) && !wallStatus[1];
    public bool ShouldWallClimb => wallStatus[1];

    private WallDetectionLayer[] detectionlayers;
    [SerializeField, ReadOnly] private bool wallRunActive;
    [SerializeField, ReadOnly] private bool wallJumpActive;
    [SerializeField, ReadOnly] private bool wallClimbActive;
    private Transform player;
    public bool RunActive => wallRunActive;
    public bool JumpActive => wallJumpActive;
    public bool ClimbActive => wallClimbActive;
    public override void Initialize()
    {
        base.Initialize();
    }

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<FirstPersonCamera>();
        jumpBehavior = this.GetComponent<JumpBehavior>();
        collisionManager = Player.Instance.modules.collisionManagement;
        rigidbodyLinker = Player.Instance.modules.rigidbodyLinker;
        rb = Player.Instance.modules.rigidbody;
        player = Player.Instance.transform.Find("Mesh");
        detectionlayers = new WallDetectionLayer[]
        {
            new WallDetectionLayer(forwardDetectionTiltAngle, backwardDetectionTiltAngle, 0, 0),
            new WallDetectionLayer(forwardDetectionTiltAngle, backwardDetectionTiltAngle, -detectionLayerSpacing, 0),
            new WallDetectionLayer(forwardDetectionTiltAngle, backwardDetectionTiltAngle, -detectionLayerSpacing/2, 0),
            new WallDetectionLayer(forwardDetectionTiltAngle, backwardDetectionTiltAngle, detectionLayerSpacing, 0),
            new WallDetectionLayer(forwardDetectionTiltAngle, backwardDetectionTiltAngle, detectionLayerSpacing/2, 0)
        };
    }

    private void FixedUpdate()
    {
        UpdateWallStatus();
        if (collisionManager.IsGrounded.old)
        {
            inAirCooldown.ResetTimer();
        }
        else
        {
            inAirCooldown.CountDown(Time.fixedDeltaTime);
        }
        entryEffectsCooldown.CountDown(Time.fixedDeltaTime);

        if(RunActive && !ShouldLeftRun && !ShouldRightRun)
        {
            coyoteTimer.CountDown(Time.fixedDeltaTime);
        }
        
        if (!RunActive)
        {
            exitEffectsCooldown.CountDown(Time.fixedDeltaTime);
            
            if (!exitEffectsCooldown.TimerActive())
            {
                cameraController.ResetUpAxis();
            }
        }
    }

    private void UpdateWallStatus()
    {
        wallStatus = new bool[] { false, false, false };
        wallNormals = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };
        foreach (WallDetectionLayer layer in detectionlayers)
        {
            layer.CheckWalls(player, rigidbodyLinker, collisionManager, wallCheckDistance, validWalls);
            (bool, Vector3) temp;
            temp = layer.LeftDetected();
            wallStatus[0] |= temp.Item1;
            if (temp.Item2 != Vector3.zero)
            {
                wallNormals[0] = temp.Item2;
            }

            temp = layer.FrontDetected();
            wallStatus[1] |= temp.Item1;
            if (temp.Item2 != Vector3.zero)
            {
                wallNormals[1] = temp.Item2;
            }

            temp = layer.RightDetected();
            wallStatus[2] |= temp.Item1;
            if (temp.Item2 != Vector3.zero)
            {
                wallNormals[2] = temp.Item2;
            }
        }
    }

    public void InitialWallClimb()
    {
        RunExit();
        wallClimbActive = true;
        cameraController.ResetUpAxis();
        StopPlayerFalling(rb, collisionManager);
        Vector3 upAlongWall = Vector3.Cross(rb.transform.right, wallNormals[1]);

        rb.AddForce(rightForce * -wallNormals[1], ForceMode.VelocityChange);
        rb.AddForce(initialClimbForce * upAlongWall, ForceMode.VelocityChange);
    }

    public static void StopPlayerFalling(Rigidbody rb, CollisionManagement collisionManager)
    {
        bool playerFalling = Vector3.Dot(rb.velocity, collisionManager.ValidUpAxis) < 0;
        if (playerFalling)
        {
            rb.velocity = rb.velocity.ProjectOnContactPlane(collisionManager.ValidUpAxis);
        }
    }

    public void RunInitial()
    {
        coyoteTimer.ResetTimer();
        if (!entryEffectsCooldown.TimerActive())
        {
            StopPlayerFalling(rb, collisionManager);
        }

        wallRunActive = true;
    }

    public void Climb()
    {
        soundManager.Play("WallClimb");
        Vector3 upAlongWall = GetUpAlongWall(wallNormals[1]);
        
        cameraController.TiltUpAxis(Vector3.Cross(-wallNormals[1], upAlongWall) * wallrunCameraTilt);

        rb.AddForce(rightForce * -wallNormals[1], ForceMode.Acceleration);
        rb.AddForce(climbForce * upAlongWall, ForceMode.Acceleration);
    }
    
    public void JumpInitial()
    {
        RunExit();
        wallJumpActive = true;
        soundManager.Play("WallJump");
        jumpHoldingTimer.ResetTimer();

        Vector3 sumNormals = Vector3.zero;
        for (int i = 0; i < wallNormals.Length; i++)
        {
            sumNormals += wallNormals[i];
        }

        wallJumpWallDirection = sumNormals.normalized;
        Vector3 cameraFaceDirection = Camera.main.transform.forward;

        jumpDirection = Vector3.Project(cameraFaceDirection, wallJumpWallDirection);
        StopPlayerFalling(rb, collisionManager);

        jumpBehavior.Jump();
        rb.AddForce(wallJumpWallDirection * againstWallPush, ForceMode.VelocityChange);
        rb.AddForce(jumpInitialPush * jumpDirection, ForceMode.VelocityChange);
    }

    public void WallJumpContinous()
    {
        jumpHoldingTimer.CountDown(Time.fixedDeltaTime);
        rb.AddForce(jumpContinousPush * jumpDirection, ForceMode.Acceleration);
        rb.AddForce(jumpContinousUpForce * collisionManager.ValidUpAxis, ForceMode.Acceleration);
    }

    private Vector3 GetUpAlongWall(Vector3 normal)
    {
        return Vector3.Cross(normal, Vector3.Cross(collisionManager.ValidUpAxis, normal));
    }

    public void RightRun()
    {
        WallRun(-wallNormals[2], GetUpAlongWall(wallNormals[2]));
    }

    public void LeftRun()
    {
        WallRun(-wallNormals[0], GetUpAlongWall(wallNormals[0]));
    }

    private void WallRun(Vector3 right, Vector3 up)
    {
        soundManager.Play("WallSkid");
        Vector3 forward = Vector3.Cross(right, up);
        if (!entryEffectsCooldown.TimerActive())
        {
            cameraController.TiltUpAxis(forward * wallrunCameraTilt);
        }

        rb.AddForce(right * rightForce, ForceMode.Acceleration);
        rb.AddForce(up * upwardsForce, ForceMode.Acceleration);
    }

    public void WallClimbExit()
    {
        StartCoroutine(soundManager.Stop("WallClimb"));
        wallClimbActive = false;
    }
    public void RunExit()
    {
        StartCoroutine(soundManager.Stop("WallSkid"));
        exitEffectsCooldown.ResetTimer();
        entryEffectsCooldown.ResetTimer();
        wallRunActive = false;
    }
    public void JumpExit()
    {
        wallJumpActive = false;
    }
}