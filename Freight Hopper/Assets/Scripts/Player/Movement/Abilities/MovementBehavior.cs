using UnityEngine;

public class MovementBehavior : AbilityBehavior
{
    [SerializeField] private float oppositeInputAngle = 170;

    [SerializeField] private Speedometer speedometer;
    [SerializeField] private VelocityController groundController;
    [SerializeField] private VelocityController airController;

    public float HorizontalSpeed => speedometer.AbsoluteHorzSpeed;
    public float Speed => rb.velocity.magnitude;

    private Transform cameraTransform;

    public override void Initialize(Rigidbody rb, SoundManager sm, PlayerAbilities pa)
    {
        base.Initialize(rb, sm, pa);
        cameraTransform = Camera.main.transform;
        speedometer.Initialize(rb);
        groundController.Initialize(rb, speedometer, oppositeInputAngle);
        airController.Initialize(rb, speedometer, oppositeInputAngle);
    }

    private Vector3 ConvertInputToCameraSpace(Vector3 forward, Vector3 right)
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
        // If the player is in the air, the default direction is upwards. So this calculation does nothing in the air.
        Vector3 surfaceMoveDirection = direction.ProjectOnContactPlane(physicsManager.collisionManager.ContactNormal.current).normalized;
        if (direction.IsZero())
        {
            return;
        }
        if (!physicsManager.collisionManager.IsGrounded.current)
        {
            airController.Move(surfaceMoveDirection);
        }
        else
        {
            physicsManager.friction.ReduceFriction(1);
            groundController.Move(surfaceMoveDirection);
        }
    }

    public void MoveAction()
    {
        Vector3 relativeInput = ConvertInputToCameraSpace(cameraTransform.forward, cameraTransform.right);

        Move(relativeInput);
    }

    public override void EntryAction()
    {
    }

    public void UpdateMovement()
    {
        speedometer.UpdateSpeedometer();
    }

    public override void Action()
    {
        MoveAction();
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