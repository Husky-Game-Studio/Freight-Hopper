using UnityEngine;

[System.Serializable, RequireComponent(typeof(Rigidbody))]
public class PhysicsManager : MonoBehaviour
{
    [SerializeField] private bool aerial = false;
    public Gravity gravity;
    public Friction friction;
    public CollisionManagement collisionManager;
    public RigidbodyLinker rigidbodyLinker;

    public Rigidbody rb;

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        collisionManager.Initialize(rb, this, friction, aerial);
        gravity.Initialize(rb, collisionManager, aerial);
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