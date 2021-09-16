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

    public void DestroyObject()
    {
        GetComponent<CartProperties>().SnapJoint();
        RigidbodyDestroyed?.Invoke();
        GameObject go = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        go.transform.localScale = Vector3.one * scale;
        Destroy(go, explosionTime);
        Destroy(this.gameObject, explosionTime);
    }
}