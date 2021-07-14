using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Destructable : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private bool explosion;
    [SerializeField] private float explosionTime;
    public float ExplosionTime => explosionTime;
    [SerializeField] private float maximumForce;
    [SerializeField] private float scale;
    private static GameObject explosionPrefab;

    public event Action RigidbodyDestroyed;

    private void Awake()
    {
        if (explosionPrefab == null)
        {
            explosionPrefab = (GameObject)Resources.Load("Projectiles/Explosion");
        }
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        float collisionMass = 1;
        if (collision.rigidbody != null)
        {
            collisionMass = collision.rigidbody.mass;
        }
        for (int i = 0; i < collision.contactCount; i++)
        {
            float strength = Vector3.Dot(collision.GetContact(i).normal, collision.relativeVelocity) * collisionMass;
            if (strength > maximumForce)
            {
                DestroyObject();
                break;
            }
        }
    }

    public void DestroyObject()
    {
        RigidbodyDestroyed?.Invoke();
        GameObject go = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        go.transform.localScale = Vector3.one * scale;
        Destroy(go, explosionTime);
        Destroy(this.gameObject);
    }
}