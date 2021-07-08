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
    [SerializeField] private float forwardForce = 5;

    [Space, Header("Wall Climbing")]
    [SerializeField] private float initialClimbForce = 5;
    [SerializeField] private float climbForce = 10;

    [Space, Header("Wall Jump")]
    [SerializeField] private float jumpIniitalPush = 10;
    [SerializeField] private float jumpContinousForce = 10;
    [SerializeField] private float jumpContinousPush = 10;

    [Space, Header("Timers")]
    public Timer coyoteTimer = new Timer(0.5f);
    public Timer jumpHoldingTimer = new Timer(0.5f);

    private Vector3 jumpNormalCache;
    private FirstPersonCamera cameraController;
    private JumpBehavior jumpBehavior;

    // left, front, right
    [SerializeField, ReadOnly] private bool[] wallStatus = new bool[3];
    [SerializeField, ReadOnly] private Vector3[] wallNormals = new Vector3[3];

    // Left, Front, Right
    public bool[] WallStatus() => wallStatus.Clone() as bool[];

    private WallDetectionLayer[] detectionlayers;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<FirstPersonCamera>();
        jumpBehavior = this.GetComponent<JumpBehavior>();
        detectionlayers = new WallDetectionLayer[]
        {
            new WallDetectionLayer(forwardDetectionTiltAngle, backwardDetectionTiltAngle, 0, 0),
            new WallDetectionLayer(forwardDetectionTiltAngle, backwardDetectionTiltAngle, -detectionLayerSpacing, 0),
            new WallDetectionLayer(forwardDetectionTiltAngle, backwardDetectionTiltAngle, detectionLayerSpacing, 0)
        };
    }

    private void FixedUpdate()
    {
        wallStatus = new bool[] { false, false, false };
        wallNormals = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };
        foreach (WallDetectionLayer layer in detectionlayers)
        {
            layer.CheckWalls(physicsManager, wallCheckDistance, validWalls);
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
        cameraController.ResetUpAxis();
        StopPlayerFalling();
        Vector3 upAlongWall = Vector3.Cross(physicsManager.rb.transform.right, wallNormals[1]);

        physicsManager.rb.AddForce(rightForce * -wallNormals[1], ForceMode.VelocityChange);
        physicsManager.rb.AddForce(initialClimbForce * upAlongWall, ForceMode.VelocityChange);
    }

    private void StopPlayerFalling()
    {
        bool playerFalling = Vector3.Dot(physicsManager.rb.velocity, physicsManager.collisionManager.ValidUpAxis) < 0;
        if (playerFalling)
        {
            physicsManager.rb.velocity = physicsManager.rb.velocity.ProjectOnContactPlane(physicsManager.collisionManager.ValidUpAxis);
        }
    }

    public void WallClimb()
    {
        soundManager.Play("WallClimb");
        Vector3 upAlongWall = GetUpAlongWall(wallNormals[1]);
        cameraController.TiltUpAxis(Vector3.Cross(-wallNormals[1], upAlongWall) * wallrunCameraTilt);

        physicsManager.rb.AddForce(rightForce * -wallNormals[1], ForceMode.Acceleration);
        physicsManager.rb.AddForce(climbForce * upAlongWall, ForceMode.Acceleration);
    }

    public void WallJumpInitial()
    {
        cameraController.ResetUpAxis();
        soundManager.Play("WallJump");

        /*Vector3 sumNormals = Vector3.zero;
        foreach (Vector3 normal in wallNormals)
        {
            sumNormals += normal;
        }
        sumNormals.Normalize();*/

        Vector3 jumpNormal = Camera.main.transform.forward;

        StopPlayerFalling();

        jumpNormalCache = jumpNormal;
        jumpBehavior.Jump();
        physicsManager.rb.AddForce(jumpIniitalPush * jumpNormalCache, ForceMode.VelocityChange);
    }

    public void WallJumpContinous()
    {
        jumpHoldingTimer.CountDownFixed();
        physicsManager.rb.AddForce(jumpContinousPush * jumpNormalCache, ForceMode.Acceleration);
        physicsManager.rb.AddForce(jumpContinousForce * physicsManager.collisionManager.ValidUpAxis, ForceMode.Acceleration);
    }

    private Vector3 GetUpAlongWall(Vector3 normal)
    {
        return Vector3.Cross(normal, Vector3.Cross(physicsManager.collisionManager.ValidUpAxis, normal));
    }

    public void RightWallRun()
    {
        WallRun(-wallNormals[2], GetUpAlongWall(wallNormals[2]));
    }

    public void LeftWallRun()
    {
        WallRun(-wallNormals[0], GetUpAlongWall(wallNormals[0]));
    }

    private void WallRun(Vector3 right, Vector3 up)
    {
        soundManager.Play("WallSkid");
        Vector3 forward = Vector3.Cross(right, up);
        cameraController.TiltUpAxis(forward * wallrunCameraTilt);
        Vector3 cameraForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, -right);
        physicsManager.rb.AddForce(cameraForward * forwardForce, ForceMode.Acceleration);
        physicsManager.rb.AddForce(right * rightForce, ForceMode.Acceleration);
        physicsManager.rb.AddForce(up * upwardsForce, ForceMode.Acceleration);
    }

    public void WallClimbExit()
    {
        ExitAction();
        base.ExitAction();
    }

    public override void ExitAction()
    {
        soundManager.Stop("WallSkid");
        cameraController.ResetUpAxis();
    }
}