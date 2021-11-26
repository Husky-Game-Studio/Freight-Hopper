using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] private Transform head;

    void Awake()
    {
        rb = this.transform.GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        rb.MovePosition(head.transform.position);
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
    void CheckDeath(Collision collision)
    {
        if (!collision.gameObject.CompareTag("landable"))
        {
            KillPlayer();
        }
    }
    

}
