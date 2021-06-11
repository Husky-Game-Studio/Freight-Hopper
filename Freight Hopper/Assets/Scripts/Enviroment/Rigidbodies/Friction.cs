using UnityEngine;

[System.Serializable]
public class Friction
{
    [System.NonSerialized] private CollisionManagement playerCollision;
    [System.NonSerialized] private Rigidbody rb;

    [SerializeField] private FrictionData defaultFriction;
    private FrictionData currentFriction;
    private float frictionReductionPercent = 1;

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

    // Expects numbers like "5% friction reduction" or 0.05f. Will only apply when grounded
    public void ReduceFriction(float percent)
    {
        frictionReductionPercent = 1 - percent;
    }

    // Resets friction reduction, meaning it doesn't occur anymore
    public void ResetFrictionReduction()
    {
        frictionReductionPercent = 1;
    }

    private void ApplyFriction()
    {
        float amount;
        if (playerCollision.IsGrounded.current)
        {
            amount = currentFriction.ground.Value * frictionReductionPercent;
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