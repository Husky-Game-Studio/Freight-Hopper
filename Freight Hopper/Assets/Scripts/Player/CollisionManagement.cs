using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Gravity))]
public class CollisionManagement : MonoBehaviour
{
    private Gravity gravity;

    [ReadOnly, SerializeField] private Rigidbody rb;

    [ReadOnly, SerializeField] private Var<bool> isGrounded;
    [ReadOnly, SerializeField] private Var<Vector3> contactNormal;
    [ReadOnly, SerializeField] private int contactCount;
    [ReadOnly, SerializeField] private int steepCount;
    [ReadOnly, SerializeField] public RigidbodyLinker rigidbodyLinker;

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
            contactNormal.current = gravity.Direction;
        }
        if (contactCount > 1)
        {
            contactNormal.current.Normalize();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        EvaulateCollisions(collision);
        if (isGrounded.current)
        {
            Landed?.Invoke();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaulateCollisions(collision);
        if (isGrounded.current && !isGrounded.old)
        {
            Landed?.Invoke();
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
            rigidbodyLinker.UpdateConnectionState(rb);

            CollisionDataCollected.Invoke();
            UpdateOldValues();
            ClearValues();
        }
    }

    private void UpdateOldValues()
    {
        isGrounded.UpdateOld();
        contactNormal.UpdateOld();
        rigidbodyLinker.UpdateOldValues();
    }

    private void ClearValues()
    {
        isGrounded.current = false;
        contactCount = 0;
        steepCount = 0;

        contactNormal.current = Vector3.zero;
        rigidbodyLinker.ClearValues();
    }
}