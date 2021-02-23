using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionCheck))]
public class Dash : MonoBehaviour
{
    private bool dashPossible = true;
    [SerializeField] private float dashForce;
    [SerializeField] private Timer delay = new Timer(0.5f);

    [Range(-90, 90)] [SerializeField] private float angle; // with 0 being level with the ground

    private bool dash = false;
    private Rigidbody rb;
    private CollisionCheck playerCollision;

    private void Start()
    {
        playerCollision = GetComponent<CollisionCheck>();
    }

    private void OnDrawGizmosSelected()
    {
        float radians = angle * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(0, Mathf.Sin(radians), Mathf.Cos(radians));
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(direction));
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        delay.CountDown();
        if (UserInput.Input.Dash() && !delay.TimerActive() && dashPossible)
        {
            dash = true;
            dashPossible = false;
            delay.ResetTimer();
        }

        if (playerCollision.IsGrounded.current)
        {
            dashPossible = true;
        }
    }

    private void FixedUpdate()
    {
        if (dash)
        {
            float radians = angle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(0, Mathf.Sin(radians), Mathf.Cos(radians));
            rb.AddForce(transform.TransformDirection(direction) * dashForce, ForceMode.Impulse);
            dash = false;
            dashPossible = false;
        }
    }
}