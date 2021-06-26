using UnityEngine;

public class WallRunBehavior : AbilityBehavior
{
    /// Front, Right, Back, Left
    [SerializeField, ReadOnly] private Vector3[] wallNormals = new Vector3[4];

    [SerializeField] private float wallrunCameraTilt = 10;
    [SerializeField] private float wallCheckDistance = 1;

    [SerializeField] private LayerMask validWalls;
    [SerializeField] private float upwardsForce = 10;
    [SerializeField] private float rightForce = 5; // force applied towards the wall
    [SerializeField] private float initialClimbForce = 5;
    [SerializeField] private float climbForce = 10;
    [SerializeField] private FrictionData wallFriction; // This should be removed in the future

    [Space]
    [SerializeField] private float jumpIniitalPush = 10;

    [SerializeField] private float jumpContinousForce = 10;

    [SerializeField] private float jumpContinousPush = 10;
    public Timer coyoteTimer = new Timer(0.5f);
    public Timer jumpHoldingTimer = new Timer(0.5f);
    private Vector3 jumpNormalCache;
    private FirstPersonCamera cameraController;
    private JumpBehavior jumpBehavior;
    private bool[] wallStatus;
    public bool[] WallStatus => wallStatus;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<FirstPersonCamera>();
        jumpBehavior = this.GetComponent<JumpBehavior>();
    }

    private void FixedUpdate()
    {
        wallStatus = CheckWalls();
    }

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

    /// <summary>
    /// Checks cardinal direction (relative) walls for their normals in range
    /// </summary>
    /// <returns>returns normals of all 4 walls</returns>
    public Vector3[] CheckWalls(float distance, LayerMask layers)
    {
        Vector3[] walls = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
        Vector3[] directions = { Vector3.forward, Vector3.right, -Vector3.forward, -Vector3.right };
        RaycastHit hit;
        for (int i = 0; i < 4; ++i)
        {
            if (Physics.Raycast(playerPM.rb.position, playerPM.rb.transform.TransformDirection(directions[i]), out hit, distance, layers))
            {
                if (!hit.transform.CompareTag("landable"))
                {
                    continue;
                }
                if (hit.rigidbody != null)
                {
                    playerPM.rigidbodyLinker.UpdateLink(hit.rigidbody);
                }
                float collisionAngle = Vector3.Angle(hit.normal, playerPM.collisionManager.ValidUpAxis);
                if (collisionAngle > playerPM.collisionManager.MaxSlope)
                {
                    walls[i] = hit.normal;
                }
            }
        }

        return walls;
    }

    /// <summary>
    /// Returns if "touching" one of the 4 cardinal walls.
    /// Also updates internal wall normal storage
    /// </summary>
    public bool[] CheckWalls()
    {
        if (playerPM.rb == null)
        {
            // Null reference errors are occuring, this is a temp fix
            bool[] falseArray = { false, false, false, false };
            return falseArray;
        }
        return UpdateWallStatus(CheckWalls(wallCheckDistance, validWalls));
    }

    public override void EntryAction()
    {
    }

    public override void Action()
    {
        //playerPM.friction.ReduceFriction(3);
    }

    public void InitialWallClimb()
    {
        cameraController.ResetUpAxis();
        if (Vector3.Dot(playerPM.rb.velocity, playerPM.collisionManager.ValidUpAxis) < 0)
        {
            playerPM.rb.velocity = playerPM.rb.velocity.ProjectOnContactPlane(playerPM.collisionManager.ValidUpAxis);
        }
        Vector3 upAlongWall = Vector3.Cross(playerPM.rb.transform.right, wallNormals[0]);
        playerPM.rb.AddForce(rightForce * -wallNormals[0], ForceMode.VelocityChange);
        playerPM.rb.AddForce(initialClimbForce * upAlongWall, ForceMode.VelocityChange);
    }

    public void WallClimb()
    {
        playerSM.Play("WallClimb");
        Vector3 upAlongWall = GetUpAlongWall(wallNormals[0]);
        cameraController.TiltUpAxis(Vector3.Cross(-wallNormals[0], upAlongWall) * wallrunCameraTilt);
        playerPM.rb.AddForce(rightForce * -wallNormals[0], ForceMode.Acceleration);
        playerPM.rb.AddForce(climbForce * upAlongWall, ForceMode.Acceleration);
    }

    public void WallJumpInitial()
    {
        cameraController.ResetUpAxis();
        playerSM.Play("WallJump");
        Vector3 sumNormals = Vector3.zero;
        foreach (Vector3 normal in wallNormals)
        {
            sumNormals += normal;
        }
        sumNormals.Normalize();
        if (Vector3.Dot(playerPM.rb.velocity, playerPM.collisionManager.ValidUpAxis) < 0)
        {
            playerPM.rb.velocity = playerPM.rb.velocity.ProjectOnContactPlane(playerPM.collisionManager.ValidUpAxis);
        }

        jumpNormalCache = sumNormals;
        jumpBehavior.Jump();
        playerPM.rb.AddForce(jumpIniitalPush * sumNormals, ForceMode.VelocityChange);
    }

    public void WallJumpContinous()
    {
        jumpHoldingTimer.CountDownFixed();
        playerPM.rb.AddForce(jumpContinousPush * jumpNormalCache, ForceMode.Acceleration);
        playerPM.rb.AddForce(jumpContinousForce * playerPM.collisionManager.ValidUpAxis, ForceMode.Acceleration);
    }

    private Vector3 GetUpAlongWall(Vector3 normal)
    {
        return Vector3.Cross(normal, Vector3.Cross(playerPM.collisionManager.ValidUpAxis, normal));
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
        playerSM.Play("WallSkid");
        cameraController.TiltUpAxis(Vector3.Cross(right, up) * wallrunCameraTilt);
        playerPM.rb.AddForce(right * rightForce, ForceMode.Acceleration);
        playerPM.rb.AddForce(up * upwardsForce, ForceMode.Acceleration);
    }

    public void WallClimbExit()
    {
        ExitAction();
        base.ExitAction();
    }

    public override void ExitAction()
    {
        playerSM.Stop("WallSkid");
        cameraController.ResetUpAxis();
    }
}