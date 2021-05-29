using UnityEngine;

[System.Serializable]
public class Friction
{
    private CollisionManagement playerCollision;
    private Rigidbody rb;

    [SerializeField] private FrictionData defaultFriction;

    public void Initialize(Rigidbody rb, CollisionManagement collisionManagement)
    {
        playerCollision = collisionManagement;
        this.rb = rb;
        playerCollision.CollisionDataCollected += ApplyFriction;
    }

    ~Friction()
    {
        playerCollision.CollisionDataCollected -= ApplyFriction;
    }

    private void ApplyFriction()
    {
        float amount = playerCollision.IsGrounded.current ? defaultFriction.Ground : defaultFriction.Air;

        Vector3 force = (rb.velocity - playerCollision.rigidbodyLinker.ConnectionVelocity.current) * amount;

        if (playerCollision.IsGrounded.current)
        {
            force = force.ProjectOnContactPlane(playerCollision.ContactNormal.current);
        }

        rb.AddForce(-force, ForceMode.VelocityChange);
    }
}