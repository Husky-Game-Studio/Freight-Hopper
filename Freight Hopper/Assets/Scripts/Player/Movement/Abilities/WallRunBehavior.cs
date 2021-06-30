using UnityEngine;

public class WallRunBehavior : AbilityBehavior
{
    [Header("Wall detection")]
    [SerializeField] private float wallCheckDistance = 1;
    [SerializeField] private float wallrunCameraTilt = 5;
    [SerializeField] private LayerMask validWalls;

    [Space, Header("Wall Running")]
    [SerializeField] private float upwardsForce = 10;
    [SerializeField] private float rightForce = 5;

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

    // Front, Right, Back, Left is what the array represents
    [SerializeField, ReadOnly] private Vector3[] wallNormals = new Vector3[4];
    private bool[] wallStatus;

    public bool[] WallStatus() => wallStatus.Clone() as bool[];

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<FirstPersonCamera>();
        jumpBehavior = this.GetComponent<JumpBehavior>();
    }

    private void FixedUpdate()
    {
        wallStatus = CheckWalls();
    }

    // bool[] represents the relative cardinal directions. If there is a wall forward then bool[0] = true
    private bool[] UpdateWallStatus(Vector3[] walls)
    {
        bool[] nearWall = { false, false, false, false };
        for (int i = 0; i < 4; i++)
        {
            if (!walls[i].IsZero())
            {
                nearWall[i] = true;
            }
        }
        wallNormals = walls;
        return nearWall;
    }

    // Returns the 4 relative cardinal directions as normals. Vector3.zero if no wall is found in the relative cardinal direction
    public Vector3[] CheckWalls(float distance, LayerMask layers)
    {
        Vector3[] walls = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
        Vector3[] directions = { Vector3.forward, Vector3.right, -Vector3.forward, -Vector3.right };
        for (int i = 0; i < 4; ++i)
        {
            if (Physics.Raycast(physicsManager.rb.position, physicsManager.rb.transform.TransformDirection(directions[i]), out RaycastHit hit, distance, layers))
            {
                if (!hit.transform.CompareTag("landable"))
                {
                    continue;
                }
                if (hit.rigidbody != null)
                {
                    physicsManager.rigidbodyLinker.UpdateLink(hit.rigidbody);
                }
                float collisionAngle = Vector3.Angle(hit.normal, physicsManager.collisionManager.ValidUpAxis);
                if (collisionAngle > physicsManager.collisionManager.MaxSlope)
                {
                    walls[i] = hit.normal;
                }
            }
        }

        return walls;
    }

    // Returns if "touching" one of the 4 relative cardinal walls.
    // Also updates internal wall normal storage
    public bool[] CheckWalls()
    {
        return UpdateWallStatus(CheckWalls(wallCheckDistance, validWalls));
    }

    public void InitialWallClimb()
    {
        cameraController.ResetUpAxis();
        StopPlayerFalling();
        Vector3 upAlongWall = Vector3.Cross(physicsManager.rb.transform.right, wallNormals[0]);

        physicsManager.rb.AddForce(rightForce * -wallNormals[0], ForceMode.VelocityChange);
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
        Vector3 upAlongWall = GetUpAlongWall(wallNormals[0]);
        cameraController.TiltUpAxis(Vector3.Cross(-wallNormals[0], upAlongWall) * wallrunCameraTilt);

        physicsManager.rb.AddForce(rightForce * -wallNormals[0], ForceMode.Acceleration);
        physicsManager.rb.AddForce(climbForce * upAlongWall, ForceMode.Acceleration);
    }

    public void WallJumpInitial()
    {
        cameraController.ResetUpAxis();
        soundManager.Play("WallJump");

        Vector3 sumNormals = Vector3.zero;
        foreach (Vector3 normal in wallNormals)
        {
            sumNormals += normal;
        }
        sumNormals.Normalize();

        StopPlayerFalling();

        jumpNormalCache = sumNormals;
        jumpBehavior.Jump();
        physicsManager.rb.AddForce(jumpIniitalPush * sumNormals, ForceMode.VelocityChange);
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
        WallRun(-wallNormals[1], GetUpAlongWall(wallNormals[1]));
    }

    public void LeftWallRun()
    {
        WallRun(-wallNormals[3], GetUpAlongWall(wallNormals[3]));
    }

    private void WallRun(Vector3 right, Vector3 up)
    {
        soundManager.Play("WallSkid");
        cameraController.TiltUpAxis(Vector3.Cross(right, up) * wallrunCameraTilt);
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