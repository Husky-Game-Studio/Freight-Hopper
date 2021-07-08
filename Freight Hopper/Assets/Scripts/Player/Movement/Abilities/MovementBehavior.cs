using UnityEngine;

public class MovementBehavior : AbilityBehavior
{
    [SerializeField, ReadOnly] private float speed;
    [SerializeField, ReadOnly] private Vector3 horizontalMomentum;
    [SerializeField, ReadOnly] private float horizontalMomentumSpeed;
    [Space]
    [SerializeField, ReadOnly] private Vector3 horizontalMomentumRelative;
    [SerializeField, ReadOnly] private float horizontalMomentumRelativeSpeed;

    [SerializeField] private float oppositeInputAngle = 170;

    [SerializeField] private float groundAcceleration = 20;
    [SerializeField] private float airAcceleration = 10;
    [SerializeField] private float groundAngleChange = 1;
    [SerializeField] private float airAngleChange = 1;
    [SerializeField] private float groundSpeedLimit = 15;
    [SerializeField] private float airSpeedLimit = 5;

    private Transform cameraTransform;
    public float Speed => groundAcceleration;

    public override void Initialize(PhysicsManager pm, SoundManager sm, PlayerAbilities pa)
    {
        base.Initialize(pm, sm, pa);
        cameraTransform = Camera.main.transform;
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
            if (OppositeInput(horizontalMomentum, relativeDirection) || horizontalMomentumSpeed < airSpeedLimit)
            {
                physicsManager.rb.AddForce(relativeDirection * airAcceleration, ForceMode.Acceleration);
            }
            else
            {
                physicsManager.rb.AddForce(-horizontalMomentum, ForceMode.VelocityChange);
                Vector3 rotatedVector = Vector3.RotateTowards(horizontalMomentum.normalized, relativeDirection, airAngleChange, 0);
                physicsManager.rb.AddForce(rotatedVector * horizontalMomentumSpeed, ForceMode.VelocityChange);
            }
        }
        else
        {
            physicsManager.friction.ReduceFriction(1);
            float acceleration = groundAcceleration;
            float nextSpeed = ((acceleration * Time.fixedDeltaTime * relativeDirection) +
                horizontalMomentumRelative).magnitude;

            if (nextSpeed < groundSpeedLimit)
            {
                physicsManager.rb.AddForce(relativeDirection * acceleration, ForceMode.Acceleration);
            }
            else
            {
                physicsManager.rb.AddForce(-horizontalMomentumRelative, ForceMode.VelocityChange);
                Vector3 rotatedVector = Vector3.RotateTowards(horizontalMomentumRelative.normalized, relativeDirection, groundAngleChange, 0);
                physicsManager.rb.AddForce(rotatedVector * horizontalMomentumRelativeSpeed, ForceMode.VelocityChange);
            }
            if (OppositeInput(horizontalMomentumRelative, relativeDirection))
            {
                physicsManager.friction.ResetFrictionReduction();
            }
        }
    }

    private bool OppositeInput(Vector3 momentumDirection, Vector3 inputDirection)
    {
        if (inputDirection.IsZero())
        {
            return false;
        }
        Vector3 normalizedMomentumDirection = momentumDirection.normalized;
        return Vector3.Angle(normalizedMomentumDirection, inputDirection) > oppositeInputAngle;
    }

    private void FixedUpdate()
    {
    }

    public void PlayerMove()
    {
        Vector3 relativeMove = RelativeMove(cameraTransform.forward, cameraTransform.right);

        Move(relativeMove);
    }

    public override void EntryAction()
    {
    }

    public void UpdateSpeedometer()
    {
        if (UserInput.Instance.Move().IsZero())
        {
            physicsManager.friction.ResetFrictionReduction();
        }
        Vector3 velocity = physicsManager.rb.velocity;
        speed = velocity.magnitude;

        horizontalMomentum = velocity.ProjectOnContactPlane(physicsManager.collisionManager.ContactNormal.current);
        horizontalMomentumSpeed = horizontalMomentum.magnitude;
        horizontalMomentumRelative = horizontalMomentum - physicsManager.rigidbodyLinker.ConnectionVelocity.current;
        horizontalMomentumRelativeSpeed = horizontalMomentumRelative.magnitude;
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