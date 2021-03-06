using UnityEngine;

[System.Serializable]
public class DoubleJump
{
    [Range(25, 300)]
    [SerializeField] private float percentStrengthComparedToNormalJump = 75;

    [SerializeField, ReadOnly] private bool doubleJumpReady;
    [SerializeField, ReadOnly] private bool jumpInputLetGo = false;

    private JumpBehavior jumpBehavior;
    private CollisionCheck playerCollision;

    public void Initialize(CollisionCheck playerCollision, JumpBehavior jumpBehavior)
    {
        this.jumpBehavior = jumpBehavior;
        this.playerCollision = playerCollision;
    }

    public void Landed()
    {
        doubleJumpReady = true;
    }

    public void TryDoubleJump()
    {
        if (!playerCollision.IsGrounded.current && doubleJumpReady && !jumpBehavior.CanJump && jumpInputLetGo)
        {
            jumpInputLetGo = false;
            doubleJumpReady = false;
            jumpBehavior.Jump(jumpBehavior.JumpHeight * (percentStrengthComparedToNormalJump / 100));
        }
        else if (doubleJumpReady)
        {
            jumpInputLetGo = true;
        }
    }
}