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
        // Gets the x and y axis of the player input and puts it in a vector

        Vector2 input = new Vector2(UserInput.Input.Move().x, UserInput.Input.Move().y);
        input = Vector2.ClampMagnitude(input, 1);

        desiredVelocity = new Vector3(input.x, 0f, input.y) * playerMoveSpeedLimit;
    }

    private void FixedUpdate()
    {
        // Gets camera forward and right vector
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        // Sets the y axis to 0 so there is no conflict
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraRight.Normalize();
        cameraForward.Normalize();
        // Moves relative to the camera

        Vector3 relativeMove = cameraForward * desiredVelocity.z + cameraRight * desiredVelocity.x;

        Move(relativeMove);

        // changes the forward vector
        if (relativeMove != Vector3.zero)
        {
            gameObject.transform.forward = relativeMove;
        }
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
        float newX = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);

        rb.velocity = velocity;
    }
}