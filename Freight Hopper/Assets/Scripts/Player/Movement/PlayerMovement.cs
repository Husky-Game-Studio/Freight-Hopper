using UnityEngine;

[RequireComponent(typeof(CollisionManagement), typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public MovementBehavior movementBehavior;
    public JumpBehavior jumpBehavior;
    public GroundPoundBehavior groundPoundBehavior;
    public GrapplePoleBehavior grapplePoleBehavior;
    public DoubleJumpBehavior doubleJumpBehavior;
    public FullStopBehavior fullstopBehavior;

    private Rigidbody rb;
    private CollisionManagement playerCollision;

    private void OnValidate()
    {
        movementBehavior = GetComponentInChildren<MovementBehavior>();
        jumpBehavior = GetComponentInChildren<JumpBehavior>();
        groundPoundBehavior = GetComponentInChildren<GroundPoundBehavior>();
        grapplePoleBehavior = GetComponentInChildren<GrapplePoleBehavior>();
        doubleJumpBehavior = GetComponentInChildren<DoubleJumpBehavior>();
        fullstopBehavior = GetComponentInChildren<FullStopBehavior>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollision = GetComponent<CollisionManagement>();

        movementBehavior.Initialize(rb, playerCollision);
        doubleJumpBehavior.Initialize(playerCollision, jumpBehavior);
        jumpBehavior.Initialize(rb, playerCollision);
        groundPoundBehavior.Initalize(rb, playerCollision);
        grapplePoleBehavior.Initialize(rb, playerCollision);
        fullstopBehavior.Initialize(rb, playerCollision);
    }
}