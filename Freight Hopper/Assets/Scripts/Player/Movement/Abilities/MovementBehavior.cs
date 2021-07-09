using UnityEngine;

public class MovementBehavior : AbilityBehavior
{
    [SerializeField] private float oppositeInputAngle = 170;

    [SerializeField] private VelocityController groundController;
    [SerializeField] private VelocityController airController;

    private Transform cameraTransform;

    public override void Initialize(PhysicsManager pm, SoundManager sm, PlayerAbilities pa)
    {
        base.Initialize(pm, sm, pa);
        cameraTransform = Camera.main.transform;
        groundController.Initialize(pm, oppositeInputAngle);
        airController.Initialize(pm, oppositeInputAngle);
    }

    private Vector3 RelativeMove(Vector3 forward, Vector3 right)
    {
        forward = forward.ProjectOnContactPlane(physicsManager.collisionManager.ValidUpAxis);
        right = right.ProjectOnContactPlane(physicsManager.collisionManager.ValidUpAxis);

        // Moves relative to the camera
        Vector3 input = UserInput.Instance.Move();
        Vector3 move = (forward * input.z) + (right * input.x);
        return move;
    }

    public void Move(Vector3 direction)
    {
        Vector3 relativeDirection = direction.ProjectOnContactPlane(physicsManager.collisionManager.ContactNormal.current).normalized;
        if (direction.IsZero())
        {
            return;
        }
        if (!physicsManager.collisionManager.IsGrounded.current)
        {
            airController.Move(relativeDirection);
        }
        else
        {
            physicsManager.friction.ReduceFriction(1);
            groundController.Move(relativeDirection);
        }
    }

    public void PlayerMove()
    {
        Vector3 relativeMove = RelativeMove(cameraTransform.forward, cameraTransform.right);

        Move(relativeMove);
    }

    public override void EntryAction()
    {
    }

    public void UpdateMovement()
    {
        if (UserInput.Instance.Move().IsZero())
        {
            physicsManager.friction.ResetFrictionReduction();
        }
        groundController.UpdateSpeedometer();
        airController.UpdateSpeedometer();
    }

    public override void Action()
    {
        PlayerMove();
        if (physicsManager.collisionManager.IsGrounded.current)
        {
            soundManager.PlayRandom("Move", 7);
            soundManager.PlayRandom("Stone", 5);
        }
    }

    public override void ExitAction()
    {
    }
}