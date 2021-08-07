using System.Collections;
using UnityEngine;

[System.Serializable]
public class CollisionManagement
{
    [System.NonSerialized] private MonoBehaviour component;
    [System.NonSerialized] private Rigidbody rb;
    [System.NonSerialized] private Friction frictionManager;
    [System.NonSerialized] private bool aerial;

    [SerializeField] private float maxSlope = 60;
    [ReadOnly, SerializeField] private AutomaticTimer unlandableSurfaceDuration;
    [ReadOnly, SerializeField] private Memory<bool> isGrounded;
    [ReadOnly, SerializeField] private Memory<Vector3> contactNormal;
    [ReadOnly, SerializeField] private Memory<Vector3> velocity;
    [ReadOnly, SerializeField] private Memory<Vector3> position;
    [ReadOnly, SerializeField] private int contactCount;
    [ReadOnly, SerializeField] private int steepCount;
    [ReadOnly, SerializeField] public RigidbodyLinker rigidbodyLinker;
    [ReadOnly, SerializeField] private Vector3 validUpAxis;

    public Vector3 ValidUpAxis => validUpAxis;
    public Memory<Vector3> ContactNormal => contactNormal;
    public Memory<bool> IsGrounded => isGrounded;
    public bool LevelSurface => this.ValidUpAxis == this.ContactNormal.current && isGrounded.current;
    public Memory<Vector3> Velocity => velocity;
    public Memory<Vector3> Position => position;
    public float MaxSlope => maxSlope;

    public delegate void CollisionEventHandler();

    public event CollisionEventHandler Landed;

    public event CollisionEventHandler CollisionDataCollected;

    private Vector3 upAxis;

    public void Initialize(Rigidbody rb, MonoBehaviour component, RigidbodyLinker linker, Friction frictionManager, bool aerial)
    {
        this.rb = rb;
        this.component = component;
        this.frictionManager = frictionManager;
        this.aerial = aerial;
        this.rigidbodyLinker = linker;

        upAxis = CustomGravity.GetUpAxis(rb.position);
        contactNormal.current = upAxis;
        contactNormal.UpdateOld();

        component.StartCoroutine(LateFixedUpdate());
        unlandableSurfaceDuration.Subscribe(KillPlayer);
    }

    ~CollisionManagement()
    {
        unlandableSurfaceDuration.Unsubscribe(KillPlayer);
    }

    private bool touchedUnlandable = false;

    private void KillPlayer()
    {
        LevelController.Instance.Respawn();
    }

    private void EvaulateCollisions(Collision collision)
    {
        if (aerial)
        {
            return;
        }

        contactNormal.current = Vector3.zero;

        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;

            if (!collision.gameObject.CompareTag("landable") && rb.gameObject.CompareTag("Player"))
            {
                SurfaceProperties surfaceProperties = collision.gameObject.GetComponent<SurfaceProperties>();
                if (surfaceProperties != null && surfaceProperties.KillInstantly)
                {
                    KillPlayer();
                }
                touchedUnlandable = true;
                continue;
            }

            float collisionAngle = Vector3.Angle(normal, upAxis);
            if (collisionAngle <= maxSlope)
            {
                isGrounded.current = true;
                contactNormal.current += normal;
                contactCount++;
                frictionManager.EvalauteSurface(collision);
                rigidbodyLinker.UpdateLink(collision.rigidbody);
            }
            else
            {
                steepCount++;
                if (contactCount == 0)
                {
                    rigidbodyLinker.UpdateLink(collision.rigidbody);
                }
            }
        }
        if (contactCount == 0)
        {
            contactNormal.current = upAxis;
        }
        if (contactCount > 1)
        {
            contactNormal.current.Normalize();
        }
    }

    // Call using OnCollisionEnter from a monobehavior
    public void CollisionEnter(Collision collision)
    {
        EvaulateCollisions(collision);
        if (isGrounded.current)
        {
            Landed?.Invoke();
        }
    }

    // Call using OnCollisionStay from a monobehavior
    public void CollisionStay(Collision collision)
    {
        EvaulateCollisions(collision);
        if (isGrounded.current && !isGrounded.old)
        {
            Landed?.Invoke();
        }
    }

    private IEnumerator LateFixedUpdate()
    {
        while (true)
        {
            upAxis = CustomGravity.GetUpAxis(rb.position);
            yield return new WaitForFixedUpdate();
            rigidbodyLinker.UpdateConnectionState(rb);
            CollisionDataCollected?.Invoke();

            if (touchedUnlandable && rb.CompareTag("Player"))
            {
                unlandableSurfaceDuration.Update(Time.fixedDeltaTime);
            }
            else
            {
                unlandableSurfaceDuration.ResetTimer();
            }
            touchedUnlandable = false;
            UpdateOldValues();
            ClearValues();
        }
    }

    private void UpdateOldValues()
    {
        isGrounded.UpdateOld();
        contactNormal.UpdateOld();
        rigidbodyLinker.UpdateOldValues();
        velocity.UpdateOld();
        position.UpdateOld();
    }

    private void ClearValues()
    {
        isGrounded.current = false;
        contactCount = 0;
        steepCount = 0;
        velocity.current = rb.velocity;
        position.current = rb.position;

        if (upAxis != Vector3.zero)
        {
            validUpAxis = upAxis;
        }
        contactNormal.current = validUpAxis;
        rigidbodyLinker.ClearValues();
        frictionManager.ClearValues();
    }
}