using UnityEngine;

[RequireComponent(typeof(Gravity), typeof(CollisionCheck))]
public class MovementBehavior : MonoBehaviour
{
    private Rigidbody rb;
    private Transform cameraTransform;
    private CollisionCheck playerCollision;

    private Vector3 desiredVelocity;

    [SerializeField] private float maxAcceleration = 20;
    [SerializeField] private float maxAirAcceleration = 10;

    private Gravity gravity;
    public float Speed => playerMoveSpeedLimit;

    [SerializeField] private float playerMoveSpeedLimit;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        playerCollision = GetComponent<CollisionCheck>();
        gravity = GetComponent<Gravity>();
    }

    private void Update()
    {
        desiredVelocity = Input() * playerMoveSpeedLimit;
    }

    private void FixedUpdate()
    {
        Vector3 relativeMove = RelativeMove(cameraTransform.forward, cameraTransform.right);

        Move(relativeMove);

        // Changes the forward vector of the player to match the direction moved
        if (relativeMove != Vector3.zero)
        {
            gameObject.transform.forward = relativeMove;
        }
    }

    private Vector3 Input()
    {
        return new Vector3(UserInput.Input.Move().x, 0f, UserInput.Input.Move().y);
    }

    private Vector3 RelativeMove(Vector3 forward, Vector3 right)
    {
        forward.y = 0f;
        right.y = 0f;
        right.Normalize();
        forward.Normalize();

        // Moves relative to the camera
        Vector3 move = forward * desiredVelocity.z + right * desiredVelocity.x;
        return move;
    }

    private void Move(Vector3 desiredVelocity)
    {
        Vector3 xAxis = playerCollision.ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = playerCollision.ProjectOnContactPlane(Vector3.forward).normalized;

        Vector3 velocity = rb.velocity;

        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        float acceleration = playerCollision.IsGrounded.old ? maxAcceleration : maxAirAcceleration;

        float maxSpeedChange = acceleration * Time.deltaTime;
        float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);

        rb.velocity = velocity;
    }
}