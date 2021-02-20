using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Gravity))]
public class CollisionCheck : MonoBehaviour
{
    private Gravity gravity;

    [ReadOnly, SerializeField] private bool isGrounded;
    [SerializeField] private float maxSlope = 30;
    public bool IsGrounded => isGrounded;

    public delegate void LandedEventHandler();

    public event LandedEventHandler Landed;

    private void Awake()
    {
        gravity = GetComponent<Gravity>();
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
            for (int i = 0; i < collision.contactCount; i++)
            {
                if (collisionEffects)
                {
                    Camera.main.GetComponent<CameraDrag>().CollidDrag(-collision.GetContact(i).normal);
                }

                // Is Vector3.angle efficient?
                float collisionAngle = Vector3.Angle(collision.GetContact(i).normal, gravity.Direction);
                //Debug.Log("Angle of collision " + i + ": " + collisionAngle);
                isGrounded |= collisionAngle <= maxSlope;
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
        if (IsGrounded)
        {
            Landed.Invoke();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaulateCollisions(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("landable"))
        {
            isGrounded = false;
        }
    }
}