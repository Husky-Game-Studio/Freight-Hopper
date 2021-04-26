using UnityEngine;

public class DoubleJumpBehavior : AbilityBehavior
{
    [Range(25, 300)]
    [SerializeField] private float percentStrengthComparedToNormalJump = 75;

    //[SerializeField, ReadOnly] private bool doubleJumpReady;
    //[SerializeField, ReadOnly] private bool jumpInputLetGo = false;

    private JumpBehavior jumpBehavior;

    public void GetJumpBehavior(JumpBehavior jumpBehavior)
    {
        this.jumpBehavior = jumpBehavior;
    }

    public override void EntryAction()
    {
        jumpBehavior.Jump(jumpBehavior.JumpHeight * (percentStrengthComparedToNormalJump / 100));
    }

    public override void Action()
    {
        jumpBehavior.Action();
    }
}