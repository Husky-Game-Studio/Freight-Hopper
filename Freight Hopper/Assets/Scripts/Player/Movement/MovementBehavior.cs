using UnityEngine;

[System.Serializable]
public class MovementBehavior : MonoBehaviour
{
    [SerializeField] private float groundAcceleration = 20;
    [SerializeField] private float airAcceleration = 10;

    private Rigidbody rb;
    private CollisionManagement playerCollision;
    private Transform cameraTransform;

    private Vector3 Input => new Vector3(UserInput.Input.Move().x, 0f, UserInput.Input.Move().y);

    public float Speed => groundAcceleration;

    public void Initialize(Rigidbody rb, CollisionManagement playerCollision, Transform cameraTransform)
    {
        this.rb = rb;
        this.playerCollision = playerCollision;
        this.cameraTransform = cameraTransform;
    }

    private void OnEnable()
    {
        playerCollision.CollisionDataCollected += Movement;
    }

    private void OnDisable()
    {
        playerCollision.CollisionDataCollected -= Movement;
    }

    public void Movement()
    {
        Vector3 relativeMove = RelativeMove(cameraTransform.forward, cameraTransform.right);

        Move(rb, playerCollision, relativeMove, playerCollision.IsGrounded.current ? groundAcceleration : airAcceleration);
    }

    private Vector3 RelativeMove(Vector3 forward, Vector3 right)
    {
        forward = CollisionManagement.ProjectOnContactPlane(forward, playerCollision.ValidUpAxis);
        right = CollisionManagement.ProjectOnContactPlane(right, playerCollision.ValidUpAxis);

        // Moves relative to the camera
        Vector3 move = forward * Input.z + right * Input.x;
        return move;
    }

    public void Move(Rigidbody rigidbody, CollisionManagement collision, Vector3 direction, float acceleration)
    {
        Vector3 relativeDirection = CollisionManagement.ProjectOnContactPlane(direction, collision.ContactNormal.current).normalized;

        rigidbody.AddForce(relativeDirection * acceleration, ForceMode.Acceleration);
    }
}