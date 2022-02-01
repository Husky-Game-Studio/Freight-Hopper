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

    private CollisionManagement collisionManager;
    private Rigidbody rb;
    private RigidbodyLinker rigidbodyLinker;

    public void Initialize(Rigidbody rb, CollisionManagement cm, RigidbodyLinker rl)
    {
        this.rb = rb;
        this.collisionManager = cm;
        this.rigidbodyLinker = rl;
    }

    public void UpdateSpeedometer()
    {
        Vector3 velocity = rb.velocity;
        speed = velocity.magnitude;
        horizontalSpeed = velocity.ProjectOnContactPlane(collisionManager.ContactNormal.current).magnitude;
        relativeHorizontalVelocity = velocity.ProjectOnContactPlane(collisionManager.ContactNormal.current)
            - rigidbodyLinker.ConnectionVelocity.current;
        relativeHorizontalSpeed = relativeHorizontalVelocity.magnitude;
    }
}