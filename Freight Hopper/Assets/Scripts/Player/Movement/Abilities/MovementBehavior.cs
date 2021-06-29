using UnityEngine;

public class MovementBehavior : AbilityBehavior
{
    [SerializeField] private float groundAcceleration = 20;
    [SerializeField] private float airAcceleration = 10;
    [Space]
    [SerializeField] private float deltaIncreaseMomentumBonus;
    [SerializeField] private float decayMultiplier = 2;
    [ReadOnly, SerializeField] private Current<float> momentumBonus = new Current<float>(1);

    private Transform cameraTransform;
    public float Speed => groundAcceleration;

    public override void Initialize(PhysicsManager pm, SoundManager sm)
    {
        base.Initialize(pm, sm);
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

    public void Move(Rigidbody rigidbody, CollisionManagement collision, Vector3 direction, float acceleration)
    {
        Vector3 relativeDirection = direction.ProjectOnContactPlane(collision.ContactNormal.current).normalized;

        rigidbody.AddForce(relativeDirection * acceleration, ForceMode.Acceleration);
    }

    private void FixedUpdate()
    {
        if (UserInput.Instance.Move().z > 0)
        {
            momentumBonus.value += deltaIncreaseMomentumBonus * Time.fixedDeltaTime;
        }
        else
        {
            momentumBonus.value -= deltaIncreaseMomentumBonus * Time.fixedDeltaTime * decayMultiplier;
            momentumBonus.value = Mathf.Max(momentumBonus.Stored, momentumBonus.value);
        }
    }

    public void PlayerMove()
    {
        Vector3 relativeMove = RelativeMove(cameraTransform.forward, cameraTransform.right);

        Move(physicsManager.rb, physicsManager.collisionManager, relativeMove, physicsManager.collisionManager.IsGrounded.current ?
            groundAcceleration * momentumBonus.value : airAcceleration * momentumBonus.value);
    }

    public override void EntryAction()
    {
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