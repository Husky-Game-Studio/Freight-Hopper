using UnityEngine;

[System.Serializable]
public class Friction
{
    [System.NonSerialized] private CollisionManagement playerCollision;
    [System.NonSerialized] private Rigidbody rb;

    [SerializeField] private FrictionData defaultFriction;
    private FrictionData currentFriction;

    public void Initialize(Rigidbody rb, CollisionManagement collisionManagement)
    {
        playerCollision = collisionManagement;
        this.rb = rb;

        playerCollision.CollisionDataCollected += ApplyFriction;
        currentFriction = defaultFriction;
    }

    ~Friction()
    {
        playerCollision.CollisionDataCollected -= ApplyFriction;
    }

    private void ApplyFriction()
    {
        float amount;
        if (playerCollision.IsGrounded.current)
        {
            amount = currentFriction.ground.Value;
        }
        else
        {
            amount = currentFriction.air.Value;
        }

        Vector3 force = (rb.velocity - playerCollision.rigidbodyLinker.ConnectionVelocity.current) * amount;

        if (playerCollision.IsGrounded.current)
        {
            force = force.ProjectOnContactPlane(playerCollision.ContactNormal.current);
        }

        rb.AddForce(-force / Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    // This assumes that the surface is valid (e.g. slope angle not too steep). Called by collisionManagement
    public void EvalauteSurface(Collision collision)
    {
        SurfaceProperties surface = collision.gameObject.GetComponent<SurfaceProperties>();
        if (surface != null)
        {
            currentFriction.ground = surface.Friction;
        }
    }

    // Called by collisionManagement to reset currentfriction
    public void ClearValues()
    {
        currentFriction.ground = defaultFriction.ground;
        currentFriction.air = defaultFriction.air;
    }
}