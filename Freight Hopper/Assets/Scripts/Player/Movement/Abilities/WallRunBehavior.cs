using UnityEngine;

public class WallRunBehavior : AbilityBehavior
{
    /// Front, Right, Back, Left
    [SerializeField, ReadOnly] private Vector3[] wallNormals = new Vector3[4];

    [SerializeField] private float wallCheckDistance = 1;

    [SerializeField] private LayerMask validWalls;
    [SerializeField] private float upwardsForce = 10;
    [SerializeField] private float rightForce = 5; // force applied towards the wall
    [SerializeField] private float climbForce = 10;
    [SerializeField] private float jumpForce = 10;
    [SerializeField] private float pushAwayFromWallForce = 10;

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

    public void WallClimb()
    {
        //Debug.Log("Wall Climb");
        Vector3 upAlongWall = Vector3.Cross(playerRb.transform.right, wallNormals[0]);
        Debug.DrawLine(playerRb.position, playerRb.position + upAlongWall, Color.green, Time.fixedDeltaTime);
        playerRb.AddForce(rightForce * -wallNormals[0], ForceMode.Acceleration);
        playerRb.AddForce(climbForce * upAlongWall, ForceMode.Acceleration);
    }

    public void WallJump()
    {
        Vector3 sumNormals = Vector3.zero;
        foreach (Vector3 normal in wallNormals)
        {
            sumNormals += normal;
        }
        sumNormals.Normalize();
        playerRb.AddForce(pushAwayFromWallForce * sumNormals, ForceMode.VelocityChange);
        playerRb.AddForce(jumpForce * playerCM.ValidUpAxis, ForceMode.VelocityChange);
    }

    public void RightWallRun()
    {
        //Debug.Log("Right wall Climb");
        Vector3 upAlongWall = -Vector3.Cross(playerRb.transform.forward, wallNormals[1]);
        Debug.DrawLine(playerRb.position, playerRb.position + upAlongWall, Color.green, Time.fixedDeltaTime);
        WallRun(-wallNormals[1], upAlongWall);
    }

    public void LeftWallRun()
    {
        //Debug.Log("Left wall Climb");
        Vector3 upAlongWall = Vector3.Cross(playerRb.transform.forward, wallNormals[3]);
        Debug.DrawLine(playerRb.position, playerRb.position + upAlongWall, Color.green, Time.fixedDeltaTime);
        WallRun(-wallNormals[3], upAlongWall);
    }

    private void WallRun(Vector3 right, Vector3 up)
    {
        playerRb.AddForce(right * rightForce, ForceMode.Acceleration);
        playerRb.AddForce(up * upwardsForce, ForceMode.Acceleration);
    }

    public void WallClimbExit()
    {
        base.ExitAction();
    }

    public override void ExitAction()
    {
    }
}