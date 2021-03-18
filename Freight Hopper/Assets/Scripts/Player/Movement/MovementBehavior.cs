using UnityEngine;

[System.Serializable]
public class MovementBehavior
{
    [SerializeField] private float groundAcceleration = 20;
    [SerializeField] private float airAcceleration = 10;

    private Rigidbody rb;
    private CollisionManagement playerCollision;
    private Transform cameraTransform;
    private Transform playerTransform;
    private Transform gravity;

    private Vector3 Input => new Vector3(UserInput.Input.Move().x, 0f, UserInput.Input.Move().y);

    public float Speed => groundAcceleration;

    public void Initialize(Rigidbody rb, CollisionManagement playerCollision, Transform cameraTransform, Transform playerTransform, Transform gravity)
    {
        this.rb = rb;
        this.playerCollision = playerCollision;
        this.cameraTransform = cameraTransform;
        this.playerTransform = playerTransform;
        this.gravity = gravity;
    }

    public void Movement()
    {
        Vector3 relativeMove = RelativeMove(cameraTransform.forward, cameraTransform.right);

        Move(relativeMove);
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
        // Changes the forward vector of the player to match the direction moved
        Vector3 upAxis = CustomGravity.GetUpAxis(rb.position);

        Vector3 forward = CollisionManagement.ProjectOnContactPlane(cameraTransform.forward, upAxis).normalized;

        playerTransform.LookAt(playerTransform.position + forward, upAxis);
        gravity.up = upAxis;
        /*
                Debug.Log("Camera right projected on upaxis: " + CollisionManagement.ProjectOnContactPlane(cameraTransform.forward, upAxis).normalized);
                Debug.Log("Contact normal: " + playerCollision.ContactNormal.current);
                Debug.Log("Up axis: " + upAxis);
                Debug.Log("Player up: " + playerTransform.up);
                Debug.Log("Forward vector: " + forward);*/

        Vector3 xAxis = CollisionManagement.ProjectOnContactPlane(Vector3.right, playerCollision.ContactNormal.current).normalized;
        Vector3 zAxis = CollisionManagement.ProjectOnContactPlane(Vector3.forward, playerCollision.ContactNormal.current).normalized;

        Vector3 relativeDirection = direction.x * xAxis + zAxis * direction.z;
        float acceleration = playerCollision.IsGrounded.current ? groundAcceleration : airAcceleration;

        rb.AddForce(relativeDirection * acceleration, ForceMode.Acceleration);
    }
}