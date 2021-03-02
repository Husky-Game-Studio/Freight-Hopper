using UnityEngine;

[RequireComponent(typeof(Gravity), typeof(CollisionCheck))]
public class MovementBehavior : MonoBehaviour
{
    private Rigidbody rb;
    private Transform cameraTransform;
    [SerializeField] private Transform movementTransform;
    private CollisionCheck playerCollision;

    private Vector3 input;

    [SerializeField] private float groundAcceleration = 20;
    [SerializeField] private float airAcceleration = 10;

    private Gravity gravity;
    public float Speed => groundAcceleration;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        playerCollision = GetComponent<CollisionCheck>();
        gravity = GetComponent<Gravity>();
    }

    private void OnEnable()
    {
        playerCollision.CollisionDataCollected += Movement;
    }

    private void OnDisable()
    {
        playerCollision.CollisionDataCollected -= Movement;
    }

    private void Update()
    {
        input = Input();
    }

    private void Movement()
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
        Vector3 move = forward * input.z + right * input.x;
        return move;
    }

    private void Move(Vector3 direction)
    {
        Vector3 xAxis = playerCollision.ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = playerCollision.ProjectOnContactPlane(Vector3.forward).normalized;

        Vector3 relativeDirection = direction.x * xAxis + zAxis * direction.z;

        movementTransform.forward = playerCollision.ProjectOnContactPlane(cameraTransform.forward);

        float acceleration = playerCollision.IsGrounded.current ? groundAcceleration : airAcceleration;

        rb.AddForce(relativeDirection * acceleration, ForceMode.Acceleration);
    }
}