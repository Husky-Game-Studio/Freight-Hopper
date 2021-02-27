using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Gravity))]
public class CollisionCheck : MonoBehaviour
{
    private Gravity gravity;

    [ReadOnly, SerializeField] private Rigidbody rb;

    [ReadOnly, SerializeField] private Var<bool> isGrounded;
    [ReadOnly, SerializeField] private Var<Vector3> contactNormal;
    [ReadOnly, SerializeField] private int contactCount;
    [ReadOnly, SerializeField] private int steepCount;
    [ReadOnly, SerializeField] private Var<Rigidbody> connectedRb = new Var<Rigidbody>(null, null);
    [ReadOnly, SerializeField] private Vector3 connectionWorldPosition;
    [ReadOnly, SerializeField] private Vector3 connectionLocalPosition;
    [ReadOnly, SerializeField] private Var<Vector3> connectionVelocity;
    [ReadOnly, SerializeField] private Var<Vector3> connectionAcceleration;

    public Var<Rigidbody> ConnectedRb => connectedRb;
    public Var<Vector3> ConnectionAcceleration => connectionAcceleration;
    public Var<Vector3> ConnectionVelocity => connectionVelocity;
    public Var<Vector3> ContactNormal => contactNormal;
    public Var<bool> IsGrounded => isGrounded;

    [SerializeField] private float airFriction = 0.01f;
    [SerializeField] private float kineticGroundFriction = 0.08f;
    [SerializeField] private float maxSlope = 30;

    public delegate void LandedEventHandler();

    public event LandedEventHandler Landed;

    private void Awake()
    {
        gravity = GetComponent<Gravity>();
        rb = GetComponent<Rigidbody>();
        contactNormal.current = gravity.Direction;
        contactNormal.UpdateOld();
    }

    // This needs to be replaced by a level manager, doesn't belong here
    public void Respawn()
    {
        this.transform.position = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelSettings>().SpawnPosition;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private void EvaulateCollisions(Collision collision, bool collisionEffects = false)
    {
        if (collision.gameObject.CompareTag("landable"))
        {
            contactNormal.current = Vector3.zero;
            for (int i = 0; i < collision.contactCount; i++)
            {
                if (collisionEffects)
                {
                    Camera.main.GetComponent<CameraDrag>().CollidDrag(-collision.GetContact(i).normal);
                }

                Vector3 normal = collision.GetContact(i).normal;

                // Is Vector3.angle efficient?
                float collisionAngle = Vector3.Angle(normal, gravity.Direction);
                // Debug.Log("Angle of collision " + i + ": " + collisionAngle);
                if (collisionAngle <= maxSlope)
                {
                    isGrounded.current = true;
                    contactNormal.current += normal;
                    contactCount++;
                    connectedRb.current = collision.rigidbody;
                }
                else
                {
                    steepCount++;
                    if (contactCount == 0)
                    {
                        connectedRb.current = collision.rigidbody;
                    }
                }
            }
            if (contactCount > 1)
            {
                contactNormal.current.Normalize();
            }
            if (contactCount == 0)
            {
                contactNormal.current = gravity.Direction;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("landable"))
        {
            Respawn();
        }
        EvaulateCollisions(collision, true);
        if (isGrounded.current)
        {
            Landed.Invoke();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaulateCollisions(collision);
        if (isGrounded.current && !isGrounded.old)
        {
            Landed.Invoke();
        }
    }

    private void UpdateConnectionState()
    {
        if (connectedRb.current == connectedRb.old)
        {
            connectionVelocity.current = (connectedRb.current.transform.TransformPoint(connectionLocalPosition) - connectionWorldPosition) / Time.fixedDeltaTime;
            connectionAcceleration.current = (connectionVelocity.current - connectionVelocity.old);
            rb.AddForce(connectionAcceleration.current, ForceMode.VelocityChange);
        }

        connectionWorldPosition = rb.position;
        connectionLocalPosition = connectedRb.current.transform.InverseTransformPoint(connectionWorldPosition);
    }

    private void Friction()
    {
        float amount = isGrounded.current ? kineticGroundFriction : airFriction;

        Vector3 force = (rb.velocity - connectionVelocity.current) * amount;
        if (isGrounded.current)
        {
            force = ProjectOnContactPlane(force);
        }

        rb.AddForce(-force, ForceMode.VelocityChange);
    }

    /// <summary>
    /// This sets isGrounded to false at the start of fixed update, change script execution priority if you have issues with isGrounded of other scripts
    /// </summary>
    private void FixedUpdate()
    {
        if (connectedRb.current)
        {
            if (connectedRb.current.isKinematic || connectedRb.current.mass >= rb.mass)
            {
                UpdateConnectionState();
            }
        }

        Friction();

        isGrounded.UpdateOld();
        isGrounded.current = false;
        contactCount = 0;
        steepCount = 0;
        contactNormal.UpdateOld();

        connectionAcceleration.UpdateOld();
        connectionAcceleration.current = Vector3.zero;
        connectionVelocity.UpdateOld();
        connectionVelocity.current = Vector3.zero;
        contactNormal.current = gravity.Direction;

        if (connectedRb.current == null && connectedRb.old != null)
        {
        }
        connectedRb.UpdateOld();
        connectedRb.current = null;
    }

    public Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal.old * Vector3.Dot(vector, contactNormal.old);
    }
}