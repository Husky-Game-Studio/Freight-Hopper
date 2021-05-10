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
    public Timer climbTimer = new Timer(0.5f);

    [Space]
    [SerializeField] private float jumpIniitalPush = 10;

    [SerializeField] private float jumpContinousForce = 10;

    [SerializeField] private float jumpContinousPush = 10;
    public Timer coyoteTimer = new Timer(0.5f);
    public Timer jumpHoldingTimer = new Timer(0.5f);
    private Vector3 jumpNormalCache;
    private FirstPersonCamera cameraController;
    private JumpBehavior jumpBehavior;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<FirstPersonCamera>();
        jumpBehavior = playerRb.GetComponentInChildren<JumpBehavior>();
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
    /// Returns if "touching" one of the 4 cardinal walls.
    /// Also updates internal wall normal storage
    /// </summary>
    public bool[] CheckWalls()
    {
        return UpdateWallStatus(playerCM.CheckWalls(wallCheckDistance, validWalls));
    }

    public override void EntryAction()
    {
    }

    public override void Action()
    {
    }

    public void InitialWallClimb()
    {
        cameraController.ResetUpAxis();
        playerSM.Play("Jump");
        climbTimer.ResetTimer();
        if (Vector3.Dot(playerRb.velocity, playerCM.ValidUpAxis) < 0)
        {
            playerRb.velocity = playerRb.velocity.ProjectOnContactPlane(playerCM.ValidUpAxis);
        }
        Vector3 upAlongWall = Vector3.Cross(playerRb.transform.right, wallNormals[0]);
        playerRb.AddForce(rightForce * -wallNormals[0], ForceMode.VelocityChange);
        playerRb.AddForce(initialClimbForce * upAlongWall, ForceMode.VelocityChange);
    }

    public void WallClimb()
    {
        playerSM.Play("WallSkid");
        climbTimer.CountDownFixed();
        Vector3 upAlongWall = GetUpAlongWall(wallNormals[0]);
        cameraController.TiltUpAxis(Vector3.Cross(-wallNormals[0], upAlongWall) * wallrunCameraTilt);
        playerRb.AddForce(rightForce * -wallNormals[0], ForceMode.Acceleration);
        playerRb.AddForce(climbForce * upAlongWall, ForceMode.Acceleration);
    }

    public void WallJumpInitial()
    {
        cameraController.ResetUpAxis();
        playerSM.Play("Jump");
        Vector3 sumNormals = Vector3.zero;
        foreach (Vector3 normal in wallNormals)
        {
            sumNormals += normal;
        }
        sumNormals.Normalize();
        if (Vector3.Dot(playerRb.velocity, playerCM.ValidUpAxis) < 0)
        {
            playerRb.velocity = playerRb.velocity.ProjectOnContactPlane(playerCM.ValidUpAxis);
        }

        jumpNormalCache = sumNormals;
        jumpBehavior.Jump();
        playerRb.AddForce(jumpIniitalPush * sumNormals, ForceMode.VelocityChange);
    }

    public void WallJumpContinous()
    {
        jumpHoldingTimer.CountDownFixed();
        playerRb.AddForce(jumpContinousPush * jumpNormalCache, ForceMode.Acceleration);
        playerRb.AddForce(jumpContinousForce * playerCM.ValidUpAxis, ForceMode.Acceleration);
    }

    private Vector3 GetUpAlongWall(Vector3 normal)
    {
        return Vector3.Cross(normal, Vector3.Cross(playerCM.ValidUpAxis, normal));
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
        playerRb.AddForce(right * rightForce, ForceMode.Acceleration);
        playerRb.AddForce(up * upwardsForce, ForceMode.Acceleration);
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