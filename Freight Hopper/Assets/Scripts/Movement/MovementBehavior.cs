using UnityEngine;

public class MovementBehavior : MonoBehaviour
{
    private Rigidbody rb;
    private Transform cameraTransform;

    // Direction to move player
    private Vector3 moveDirection;

    // Speed
    [SerializeField] private float speed = 20f;

    public float Speed => speed;

    [SerializeField] private float friction;

    // Speed limit
    [SerializeField] private float playerMoveSpeedLimit;

    // Constructs the variables when the game starts
    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
    }

    // Called every frame
    private void Update()
    {
        // Gets the x and y axis of the player input and puts it in a vector
        moveDirection = new Vector3(UserInput.Input.Move().x, 0f, UserInput.Input.Move().y);
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

        // Clamps the vector to make sure the magnitude stays between 0 - playerMoveSpeedLimit
        Vector3 moveSpeedLimit = Vector3.ClampMagnitude(direction * playerMoveSpeedLimit, playerMoveSpeedLimit);

        Vector3 velocity = Vector3.ClampMagnitude(direction * speed, speed);
        //Debug.Log("Move Speed Limit " + moveSpeedLimit + " " + moveSpeedLimit.magnitude);
        //Debug.Log("Velocity " + velocity.magnitude);

        // if no input and slow enough just stop the player on the axis.
        if (rb.velocity.x < playerMoveSpeedLimit && direction.x == 0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        }
        if (rb.velocity.z < playerMoveSpeedLimit && direction.z == 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
        }

        // I am not happy how this function works, but it works so I can't complain.
        // if x and z velocity is less than speed limit then add more speed else don't
        if (Mathf.Abs(rb.velocity.x) < Mathf.Abs(moveSpeedLimit.x))
        {
            rb.velocity += velocity.x * Vector3.right;
            rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -Mathf.Abs(moveSpeedLimit.x), Mathf.Abs(moveSpeedLimit.x)), rb.velocity.y, rb.velocity.z);
        }
        else if (Mathf.Sign(rb.velocity.x) == -Mathf.Sign(moveSpeedLimit.x)) // if the player is moving in the opposite direction of the speedlimit they can move backwards
        {
            rb.velocity += velocity.x * Vector3.right;
        }
        else // if the player is above the speed limit, friction will be applied for now. This functionality should be replaced in the future by collision
        {
            rb.velocity = new Vector3(rb.velocity.x * friction, rb.velocity.y, rb.velocity.z);
        }
        if (Mathf.Abs(rb.velocity.z) < Mathf.Abs(moveSpeedLimit.z))
        {
            rb.velocity += velocity.z * Vector3.forward;
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, Mathf.Clamp(rb.velocity.z, -Mathf.Abs(moveSpeedLimit.z), Mathf.Abs(moveSpeedLimit.z)));
        }
        else if (Mathf.Sign(rb.velocity.z) == -Mathf.Sign(moveSpeedLimit.z))
        {
            rb.velocity += velocity.z * Vector3.forward;
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z * friction);
        }
    }
}