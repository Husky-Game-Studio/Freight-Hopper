using UnityEngine;

public class MovementBehavior : MonoBehaviour
{
    private UserInput input;
    private Transform transform;
    private Rigidbody rb;
    private Transform cameraTransform;

    private Vector3 moveDirection;
    [SerializeField] private float speed;
    [SerializeField] private float playerMoveSpeedLimit;

    private void Awake()
    {
        speed = 10;
        input = GetComponent<UserInput>();
        transform = GetComponent<Transform>();
        cameraTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveDirection = new Vector3(input.Move().x, 0f, input.Move().y);
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraRight.Normalize();
        cameraForward.Normalize();

        Vector3 relativeMove = cameraForward * moveDirection.z + cameraRight * moveDirection.x;
        relativeMove.y = 0f;
        Move(relativeMove);

        if (relativeMove != Vector3.zero)
        {
            gameObject.transform.forward = relativeMove;
        }
    }

    private void FixedUpdate()
    {
    }

    private void Move(Vector3 direction)
    {
        direction.Normalize();
        Vector3 velocity = direction * speed;
        if (Mathf.Abs(rb.velocity.x) < playerMoveSpeedLimit)
        {
            rb.velocity += new Vector3(velocity.x, 0f, 0f);

            rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -playerMoveSpeedLimit, playerMoveSpeedLimit), rb.velocity.y, rb.velocity.z);

            if (direction.x == 0)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
            }
        }
        if (Mathf.Abs(rb.velocity.z) < playerMoveSpeedLimit)
        {
            rb.velocity += new Vector3(0, 0, velocity.z);
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, Mathf.Clamp(rb.velocity.z, -playerMoveSpeedLimit, playerMoveSpeedLimit));

            if (direction.z == 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
            }
        }
    }
}