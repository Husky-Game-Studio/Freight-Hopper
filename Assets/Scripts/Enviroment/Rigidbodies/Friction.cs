using UnityEngine;

[System.Serializable]
public class Friction : MonoBehaviour
{
    private CollisionManagement playerCollision;
    private Rigidbody rb;
    private RigidbodyLinker rigidbodyLinker;

    [SerializeField] private FrictionData defaultFriction;
    private FrictionData currentFriction;
    [SerializeField, ReadOnly] private float frictionReductionPercent = 1;

    private void Awake()
    {
        playerCollision = Player.Instance.modules.collisionManagement;
        this.rb = Player.Instance.modules.rigidbody;
        this.rigidbodyLinker = Player.Instance.modules.rigidbodyLinker;

        playerCollision.CollisionDataCollected += ApplyFriction;
        currentFriction = defaultFriction;
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

    private void FixedUpdate()
    {
        ApplyFriction();
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

        Vector3 force = (rb.velocity - rigidbodyLinker.ConnectionVelocity.current) * amount;

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