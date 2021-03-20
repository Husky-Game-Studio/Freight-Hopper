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

    private Vector3 Input => new Vector3(UserInput.Input.Move().x, 0f, UserInput.Input.Move().y);

    public float Speed => groundAcceleration;

    public void Initialize(Rigidbody rb, CollisionManagement playerCollision, Transform cameraTransform, Transform playerTransform)
    {
        this.rb = rb;
        this.playerCollision = playerCollision;
        this.cameraTransform = cameraTransform;
        this.playerTransform = playerTransform;
    }

    public void Movement()
    {
        Vector3 relativeMove = RelativeMove(cameraTransform.forward, cameraTransform.right);

        Move(relativeMove);
    }

    private Vector3 RelativeMove(Vector3 forward, Vector3 right)
    {
        forward = CollisionManagement.ProjectOnContactPlane(forward, playerCollision.ValidUpAxis);
        right = CollisionManagement.ProjectOnContactPlane(right, playerCollision.ValidUpAxis);

        // Moves relative to the camera
        Vector3 move = forward * Input.z + right * Input.x;
        return move;
    }

    private void Move(Vector3 direction)
    {
        Vector3 relativeDirection = CollisionManagement.ProjectOnContactPlane(direction, playerCollision.ContactNormal.current).normalized;
        float acceleration = playerCollision.IsGrounded.current ? groundAcceleration : airAcceleration;

        rb.AddForce(relativeDirection * acceleration, ForceMode.Acceleration);
    }
}