using System.Collections;
using UnityEngine;

[System.Serializable]
public class CollisionManagement
{
    private Rigidbody rb;
    [SerializeField] private float maxSlope = 30;
    [ReadOnly, SerializeField] private Var<bool> isGrounded;
    [ReadOnly, SerializeField] private Var<Vector3> contactNormal;
    [ReadOnly, SerializeField] private Var<Vector3> velocity;
    [ReadOnly, SerializeField] private int contactCount;
    [ReadOnly, SerializeField] private int steepCount;
    [ReadOnly, SerializeField] public RigidbodyLinker rigidbodyLinker;
    [ReadOnly, SerializeField] private Vector3 validUpAxis;

    public Vector3 ValidUpAxis => validUpAxis;
    public Var<Vector3> ContactNormal => contactNormal;
    public Var<bool> IsGrounded => isGrounded;

    public Var<Vector3> Velocity => velocity;

    public delegate void CollisionEventHandler();

    public event CollisionEventHandler Landed;

    public event CollisionEventHandler CollisionDataCollected;

    private MonoBehaviour component;

    /// <summary>
    /// Checks cardinal direction (relative) walls for their normals in range
    /// </summary>
    /// <returns>returns normals of all 4 walls</returns>
    public Vector3[] CheckWalls(float distance, LayerMask layers)
    {
        Vector3[] walls = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
        Vector3[] directions = { Vector3.forward, Vector3.right, -Vector3.forward, -Vector3.right };
        RaycastHit hit;
        for (int i = 0; i < 4; ++i)
        {
            if (Physics.Raycast(rb.position, rb.transform.TransformDirection(directions[i]), out hit, distance, layers))
            {
                if (!hit.transform.CompareTag("landable"))
                {
                    continue;
                }
                float collisionAngle = Vector3.Angle(hit.normal, this.ValidUpAxis);
                if (collisionAngle > maxSlope)
                {
                    walls[i] = hit.normal;
                }
            }
            if (walls[i] != Vector3.zero)
            {
                //Debug.DrawLine(rb.position, rb.position + (rb.transform.TransformDirection(directions[i]) * distance), Color.red, Time.fixedDeltaTime);
            }
            else
            {
                //Debug.DrawLine(rb.position, rb.position + (rb.transform.TransformDirection(directions[i]) * distance), Color.yellow, Time.fixedDeltaTime);
            }
        }

        return walls;
    }

    public void Initialize(Rigidbody rb, MonoBehaviour component)
    {
        this.rb = rb;
        this.component = component;
        contactNormal.current = CustomGravity.GetUpAxis(rb.position);
        contactNormal.UpdateOld();

        component.StartCoroutine(LateFixedUpdate());
    }

    private void EvaulateCollisions(Collision collision)
    {
        contactNormal.current = Vector3.zero;
        Vector3 upAxis = CustomGravity.GetUpAxis(rb.position);
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;

            if (!collision.gameObject.CompareTag("landable"))
            {
                LevelController.Instance.Respawn();
            }

            // Is Vector3.angle efficient?
            float collisionAngle = Vector3.Angle(normal, upAxis);
            if (collisionAngle <= maxSlope)
            {
                isGrounded.current = true;
                contactNormal.current += normal;
                contactCount++;

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
            yield return new WaitForFixedUpdate();
            rigidbodyLinker.UpdateConnectionState(rb);

            CollisionDataCollected?.Invoke();
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
    }

    private void ClearValues()
    {
        isGrounded.current = false;
        contactCount = 0;
        steepCount = 0;
        velocity.current = rb.velocity;

        Vector3 upAxis = CustomGravity.GetUpAxis(rb.position);
        if (upAxis != Vector3.zero)
        {
            validUpAxis = upAxis;
        }
        contactNormal.current = validUpAxis;
        rigidbodyLinker.ClearValues();
    }
}