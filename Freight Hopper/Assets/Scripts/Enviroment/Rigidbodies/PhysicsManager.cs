using UnityEngine;

[System.Serializable, RequireComponent(typeof(Rigidbody))]
public class PhysicsManager : MonoBehaviour
{
    [SerializeField] private bool aerial = false;
    public Gravity gravity;
    public Friction friction;
    public CollisionManagement collisionManager;
    public RigidbodyLinker rigidbodyLinker;

    [ReadOnly] public Rigidbody rb;

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Awake()
    {
    }

    private void OnEnable()
    {
        gravity.Enable();
        rb = GetComponent<Rigidbody>();
        collisionManager.Initialize(rb, this, rigidbodyLinker, friction, aerial);
        gravity.Initialize(rb, collisionManager, aerial);
        friction.Initialize(rb, collisionManager);
    }

    private void OnDisable()
    {
        gravity.Disable();
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