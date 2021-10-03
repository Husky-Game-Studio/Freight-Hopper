using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

// Help from: https://www.youtube.com/watch?v=IvT8hjy6q4o
public class Projectile : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    public float despawnDuration = 4;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(this.gameObject, despawnDuration);
    }

    private void FixedUpdate()
    {
        this.transform.forward = rb.velocity;
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(this.gameObject, 0.05f);
    }
}