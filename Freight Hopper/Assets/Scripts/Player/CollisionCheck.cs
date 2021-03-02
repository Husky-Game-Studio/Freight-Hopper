using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    [SerializeField] private float maxSlope = 30;

    public delegate void CollisionEventHandler();

    public event CollisionEventHandler Landed;

    public event CollisionEventHandler CollisionDataCollected;

    private void Awake()
    {
        gravity = GetComponent<Gravity>();
        rb = GetComponent<Rigidbody>();
        contactNormal.current = gravity.Direction;
        contactNormal.UpdateOld();

        StartCoroutine(LateFixedUpdate());
    }

    // This needs to be replaced by a level manager, doesn't belong here
    public void Respawn()
    {
        this.transform.position = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelSettings>().SpawnPosition;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private void EvaulateCollisions(Collision collision)
    {
        if (collision.gameObject.CompareTag("landable"))
        {
            contactNormal.current = Vector3.zero;
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;

                // Is Vector3.angle efficient?
                float collisionAngle = Vector3.Angle(normal, gravity.Direction);
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
        EvaulateCollisions(collision);
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

    /// <summary>
    /// Basically rotates a vector onto the contact plane. Make sure to use the CollisionDataCollected event when using this
    /// </summary>
    /// <param name="vector">vector to rotate</param>
    /// <returns>Rotated vector</returns>
    public Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal.current * Vector3.Dot(vector, contactNormal.current);
    }

    private IEnumerator LateFixedUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (connectedRb.current)
            {
                if (connectedRb.current.isKinematic || connectedRb.current.mass >= rb.mass)
                {
                    UpdateConnectionState();
                }
            }
            CollisionDataCollected.Invoke();

            UpdateOldValues();
            ClearValues();
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

    private void UpdateOldValues()
    {
        isGrounded.UpdateOld();
        contactNormal.UpdateOld();
        connectionAcceleration.UpdateOld();
        connectionVelocity.UpdateOld();
        connectedRb.UpdateOld();
    }

    private void ClearValues()
    {
        isGrounded.current = false;
        contactCount = 0;
        steepCount = 0;

        connectionAcceleration.current = Vector3.zero;
        connectionVelocity.current = Vector3.zero;
        contactNormal.current = Vector3.zero;

        connectedRb.current = null;
    }
}