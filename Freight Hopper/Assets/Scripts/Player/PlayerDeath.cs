using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private Rigidbody rb;
    private Memory<Vector3> position;
    [SerializeField] private Transform head;
    [SerializeField] private SphereCollider deathCollider;
    [SerializeField] private LayerMask deathMask;

    private void Awake()
    {
        rb = this.transform.GetComponent<Rigidbody>();
        position.current = head.transform.position;
        position.UpdateOld();
    }

    private void CheckInBetweenCollision()
    {
        position.current = head.transform.position;
        Vector3 dif = position.current - position.old;
        float dist = dif.magnitude;
        Vector3 dir = dif.normalized;
        if (Physics.SphereCast(position.old, deathCollider.radius, dir, out _, dist, deathMask))
        {
            KillPlayer();
        }
        position.UpdateOld();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(head.transform.position);
        CheckInBetweenCollision();
    }

    public void KillPlayer()
    {
        LevelController.Instance.Respawn();
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckDeath(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        CheckDeath(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        CheckDeath(collision);
    }

    private void CheckDeath(Collision collision)
    {
        if (!collision.gameObject.CompareTag("landable"))
        {
            KillPlayer();
        }
    }
}