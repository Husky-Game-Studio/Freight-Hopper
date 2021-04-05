using UnityEngine;

[System.Serializable]
public class DoubleJump : MonoBehaviour
{
    [Range(25, 300)]
    [SerializeField] private float percentStrengthComparedToNormalJump = 75;

    [SerializeField, ReadOnly] private bool doubleJumpReady;
    [SerializeField, ReadOnly] private bool jumpInputLetGo = false;

    private JumpBehavior jumpBehavior;
    private CollisionManagement playerCollision;

    public void Initialize(CollisionManagement playerCollision, JumpBehavior jumpBehavior)
    {
        this.jumpBehavior = jumpBehavior;
        this.playerCollision = playerCollision;
    }

    private void OnEnable()
    {
        //UserInput.Input.JumpInput += TryDoubleJump;
        //playerCollision.Landed += Landed;
        //playerCollision.CollisionDataCollected += JumpLetGo;
    }

    private void OnDisable()
    {
        //UserInput.Input.JumpInput -= TryDoubleJump;
        //playerCollision.Landed -= Landed;
        //playerCollision.CollisionDataCollected -= JumpLetGo;
    }

    public void Landed()
    {
        //doubleJumpReady = true;
        //jumpInputLetGo = false;
    }

    public void JumpLetGo()
    {
        //if (!playerCollision.IsGrounded.current && doubleJumpReady /*&& !jumpBehavior.CanJump*/)
        //{
        //    jumpInputLetGo = true;
        //}
    }

    public void TryDoubleJump()
    {
        //if (!playerCollision.IsGrounded.current && doubleJumpReady /*&& !jumpBehavior.CanJump*/ && jumpInputLetGo)
        //{
        //    jumpInputLetGo = false;
        //    doubleJumpReady = false;
        //    jumpBehavior.Jump(jumpBehavior.JumpHeight * (percentStrengthComparedToNormalJump / 100));
        //}
    }
}