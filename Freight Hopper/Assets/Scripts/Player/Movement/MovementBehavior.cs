using UnityEngine;

[System.Serializable]
public class MovementBehavior
{
    [SerializeField] private float groundAcceleration = 20;
    [SerializeField] private float airAcceleration = 10;

    private Rigidbody rb;
    private CollisionManagement playerCollision;
    private Gravity gravity;
    private Transform cameraTransform;
    private Transform playerTransform;

    private Vector3 Input => new Vector3(UserInput.Input.Move().x, 0f, UserInput.Input.Move().y);

    public float Speed => groundAcceleration;

    public void Initialize(Rigidbody rb, CollisionManagement playerCollision, Gravity gravity, Transform cameraTransform, Transform playerTransform)
    {
        this.rb = rb;
        this.playerCollision = playerCollision;
        this.gravity = gravity;
        this.cameraTransform = cameraTransform;
        this.playerTransform = playerTransform;
    }

    public void Movement()
    {
        Vector3 relativeMove = RelativeMove(cameraTransform.forward, cameraTransform.right);

        Move(relativeMove);

        // Changes the forward vector of the player to match the direction moved
        if (relativeMove != Vector3.zero)
        {
            playerTransform.gameObject.transform.forward = relativeMove;
        }
    }

    private Vector3 RelativeMove(Vector3 forward, Vector3 right)
    {
        forward.y = 0f;
        right.y = 0f;
        right.Normalize();
        forward.Normalize();

        // Moves relative to the camera
        Vector3 move = forward * Input.z + right * Input.x;
        return move;
    }

    private void Move(Vector3 direction)
    {
        Vector3 xAxis = playerCollision.ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = playerCollision.ProjectOnContactPlane(Vector3.forward).normalized;

        Vector3 relativeDirection = direction.x * xAxis + zAxis * direction.z;

        float acceleration = playerCollision.IsGrounded.current ? groundAcceleration : airAcceleration;

        rb.AddForce(relativeDirection * acceleration, ForceMode.Acceleration);
    }
}