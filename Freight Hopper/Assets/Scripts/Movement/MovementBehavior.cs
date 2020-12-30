using UnityEngine;

public class MovementBehavior : MonoBehaviour
{
    private UserInput input;
    private Transform transform;
    private Rigidbody rb;
    private Transform cameraTransform;

    // Direction to move player
    private Vector3 moveDirection;

    // Speed
    [SerializeField] private float speed = 20f;

    public float Speed => speed;

    // Speed limit
    [SerializeField] private float playerMoveSpeedLimit;

    // Constructs the variables when the game starts
    private void Awake()
    {
        input = GetComponent<UserInput>();
        transform = GetComponent<Transform>();
        cameraTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
    }

    // Called every frame
    private void Update()
    {
        // Gets the x and y axis of the player input and puts it in a vector
        moveDirection = new Vector3(input.Move().x, 0f, input.Move().y);
        // Gets camera forward and right vector
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        // Sets the y axis to 0 so there is no conflict
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraRight.Normalize();
        cameraForward.Normalize();
        // Moves relative to the camera
        Vector3 relativeMove = cameraForward * moveDirection.z + cameraRight * moveDirection.x;
        relativeMove.y = 0f;
        Move(relativeMove);
        // changes the forward vector
        if (relativeMove != Vector3.zero)
        {
            gameObject.transform.forward = relativeMove;
        }
    }

    private void FixedUpdate()
    {
    }

    // Function that moves the player
    private void Move(Vector3 direction)
    {
        direction.Normalize();
        // Decomposes the vector
        Vector3 decomposedVector = new Vector3(direction.x, 0f, 0f);
        // Finds the angle between the vecotrs and converts to radians
        float theta = Vector3.Angle(direction, decomposedVector);
        theta *= Mathf.Deg2Rad;
        // Sets the speed limit of each component
        float moveSpeedLimitX = playerMoveSpeedLimit * Mathf.Cos(theta);
        float moveSpeedLimitZ = playerMoveSpeedLimit * Mathf.Sin(theta);
        Vector3 velocity = direction * speed;
        // if x and z velocity is less than speed limit then add more speed else don't
        if (Mathf.Abs(rb.velocity.x) < moveSpeedLimitX)
        {
            rb.velocity += new Vector3(velocity.x, 0f, 0f);

            rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -moveSpeedLimitX, moveSpeedLimitX), rb.velocity.y, rb.velocity.z);
            // If there is no input then dead stop player
            if (direction.x == 0)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
            }
        }
        if (Mathf.Abs(rb.velocity.z) < moveSpeedLimitZ)
        {
            rb.velocity += new Vector3(0, 0, velocity.z);
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, Mathf.Clamp(rb.velocity.z, -moveSpeedLimitZ, moveSpeedLimitZ));
            // If there is no input then dead stop player
            if (direction.z == 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
            }
        }
    }
}