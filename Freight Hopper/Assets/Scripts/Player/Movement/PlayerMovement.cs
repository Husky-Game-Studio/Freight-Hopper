using UnityEngine;

[RequireComponent(typeof(CollisionCheck), typeof(Gravity), typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private MovementBehavior movement;
    [SerializeField] private JumpBehavior jumpBehavior;
    [SerializeField] private GroundPound groundPound;
    [SerializeField] private DoubleJump doubleJump;
    [SerializeField] private FullStop fullstop;

    private Rigidbody rb;
    private Gravity gravity;
    private CollisionCheck playerCollision;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gravity = GetComponent<Gravity>();
        playerCollision = GetComponent<CollisionCheck>();

        movement.Initialize(rb, playerCollision, gravity, Camera.main.transform, this.transform);
        doubleJump.Initialize(playerCollision, jumpBehavior);
        jumpBehavior.Initialize(rb, playerCollision, gravity, GetComponent<AudioSource>());
        groundPound.Initalize(rb, playerCollision, gravity);

        fullstop.Initialize(rb);
    }

    private void OnEnable()
    {
        UserInput.JumpInput += jumpBehavior.TryJump;
        UserInput.JumpInput += doubleJump.TryDoubleJump;
        UserInput.GroundPoundInput += groundPound.TryGroundPound;
        UserInput.FullStopInput += fullstop.TryFullStop;

        playerCollision.Landed += jumpBehavior.coyoteTime.ResetTimer;
        playerCollision.Landed += doubleJump.Landed;
        playerCollision.Landed += fullstop.Landed;

        playerCollision.CollisionDataCollected += movement.Movement;
        playerCollision.CollisionDataCollected += jumpBehavior.Jumping;
        playerCollision.CollisionDataCollected += groundPound.GroundPounding;
        playerCollision.CollisionDataCollected += fullstop.cooldown.CountDownFixed;
    }

    private void OnDisable()
    {
        UserInput.JumpInput -= jumpBehavior.TryJump;
        UserInput.JumpInput -= doubleJump.TryDoubleJump;
        UserInput.GroundPoundInput -= groundPound.TryGroundPound;
        UserInput.FullStopInput -= fullstop.TryFullStop;

        playerCollision.Landed -= jumpBehavior.coyoteTime.ResetTimer;
        playerCollision.Landed -= doubleJump.Landed;
        playerCollision.Landed -= fullstop.Landed;

        playerCollision.CollisionDataCollected -= movement.Movement;
        playerCollision.CollisionDataCollected -= jumpBehavior.Jumping;
        playerCollision.CollisionDataCollected -= groundPound.GroundPounding;
        playerCollision.CollisionDataCollected -= fullstop.cooldown.CountDownFixed;
    }
}