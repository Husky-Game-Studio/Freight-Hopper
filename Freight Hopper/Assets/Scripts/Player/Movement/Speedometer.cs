using UnityEngine;

[System.Serializable]
public class Speedometer
{
    [SerializeField, ReadOnly] private Vector3 velocity;
    [SerializeField, ReadOnly] private float speed;
    private float horizontalSpeed;
    [SerializeField, ReadOnly] private Vector3 relativeHorizontalVelocity;
    [SerializeField, ReadOnly] private float relativeHorizontalSpeed;

    public Vector3 HorzVelocity => relativeHorizontalVelocity;
    public float HorzSpeed => relativeHorizontalSpeed;
    public float AbsoluteHorzSpeed => horizontalSpeed;

    private PhysicsManager physicsManager;

    public void Initialize(PhysicsManager pm)
    {
        this.physicsManager = pm;
    }

    public void UpdateSpeedometer()
    {
        Vector3 velocity = physicsManager.rb.velocity;
        speed = velocity.magnitude;
        horizontalSpeed = velocity.ProjectOnContactPlane(physicsManager.collisionManager.ContactNormal.current).magnitude;
        relativeHorizontalVelocity = velocity.ProjectOnContactPlane(physicsManager.collisionManager.ContactNormal.current)
            - physicsManager.rigidbodyLinker.ConnectionVelocity.current;
        relativeHorizontalSpeed = relativeHorizontalVelocity.magnitude;
    }
}