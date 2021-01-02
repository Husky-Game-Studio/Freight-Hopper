using UnityEngine;

public class MovementBehavior : MonoBehaviour
{
    private UserInput input;
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
        cameraTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
    }

    // Called every frame
    private void Update()
    {
        // Gets the x and y axis of the player input and puts it in a vector
        moveDirection = new Vector3(input.Move().x, 0f, input.Move().y);
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
        Vector3 relativeMove = cameraForward * moveDirection.z + cameraRight * moveDirection.x;

        relativeMove.y = 0f;
        Move(relativeMove);
        // changes the forward vector
        if (relativeMove != Vector3.zero)
        {
            gameObject.transform.forward = relativeMove;
        }
    }

    // Function that moves the player
    private void Move(Vector3 direction)
    {
        direction.Normalize();
        // Creates a Vector for the player move speed limit
        Vector3 moveSpeedLimit = direction * playerMoveSpeedLimit;
        // Clamps the vector to make sure the magnitude stays between 0 - playerMoveSpeedLimit
        moveSpeedLimit = Vector3.ClampMagnitude(new Vector3(moveSpeedLimit.x, moveSpeedLimit.y, moveSpeedLimit.z), playerMoveSpeedLimit);

        Vector3 velocity = direction * speed;
        //Debug.Log("Move Speed Limit " + moveSpeedLimit);
        //Debug.Log("Velocity " + velocity);

        // if x and z velocity is less than speed limit then add more speed else don't
        if (rb.velocity.x < moveSpeedLimit.x && Mathf.Sign(direction.x) == 1)
        {
            // 2 + 2 = 4
            rb.velocity += new Vector3(velocity.x, 0f, 0f);
            // 3
            //rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -moveSpeedLimit.x, moveSpeedLimit.x), rb.velocity.y, rb.velocity.z);

            // If there is no input then dead stop player
            if (direction.x == 0)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
            }
        }
        else if (rb.velocity.x > moveSpeedLimit.x && Mathf.Sign(direction.x) == -1)
        {
            rb.velocity += new Vector3(velocity.x, 0f, 0f);

            //rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, moveSpeedLimit.x, -moveSpeedLimit.x), rb.velocity.y, rb.velocity.z);

            // If there is no input then dead stop player
            if (direction.x == 0)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
            }
        }
        if (rb.velocity.z < moveSpeedLimit.z && Mathf.Sign(direction.z) == 1)
        {
            rb.velocity += new Vector3(0, 0, velocity.z);

            //rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, Mathf.Clamp(rb.velocity.z, -moveSpeedLimit.z, moveSpeedLimit.z));

            if (direction.z == 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
            }
        }
        else if (rb.velocity.z > moveSpeedLimit.z && Mathf.Sign(direction.z) == -1) // -2 and speed limit -3
        {
            rb.velocity += new Vector3(0, 0, velocity.z);

            //rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, Mathf.Clamp(rb.velocity.z, moveSpeedLimit.z, -moveSpeedLimit.z));

            // If there is no input then dead stop player
            if (direction.z == 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
            }
        }
    }
}