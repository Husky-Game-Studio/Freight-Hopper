using UnityEngine;

[System.Serializable, RequireComponent(typeof(Rigidbody))]
public class PhysicsManager : MonoBehaviour
{
    public Gravity gravity;
    public Friction friction;
    public CollisionManagement collisionManager;
    public RigidbodyLinker rigidbodyLinker;

    private Rigidbody rb;

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        collisionManager.Initialize(rb, this);
        gravity.Initialize(rb, collisionManager);
        friction.Initialize(rb, collisionManager);
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisionManager.CollisionEnter(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        collisionManager.CollisionStay(collision);
    }
}