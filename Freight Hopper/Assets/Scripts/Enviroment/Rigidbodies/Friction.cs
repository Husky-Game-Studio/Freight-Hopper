using UnityEngine;

[System.Serializable]
public class Friction
{
    [System.NonSerialized] private CollisionManagement playerCollision;
    [System.NonSerialized] private Rigidbody rb;

    [SerializeField] private FrictionData defaultFriction;
    [SerializeField, ReadOnly] private FrictionData currentFriction;

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
            amount = currentFriction.Ground;
        }
        else
        {
            amount = currentFriction.Air;
        }

        Vector3 force = (rb.velocity - playerCollision.rigidbodyLinker.ConnectionVelocity.current) * amount;

        if (playerCollision.IsGrounded.current)
        {
            force = force.ProjectOnContactPlane(playerCollision.ContactNormal.current);
        }

        rb.AddForce(-force, ForceMode.VelocityChange);
    }

    // This assumes that the surface is valid (e.g. slope angle not too steep)
    public void EvalauteSurface(Collision collision)
    {
        SurfaceProperties surface = collision.gameObject.GetComponent<SurfaceProperties>();
        if (surface != null)
        {
            currentFriction = surface.Friction;
        }
        else
        {
            currentFriction = defaultFriction;
        }
    }
}