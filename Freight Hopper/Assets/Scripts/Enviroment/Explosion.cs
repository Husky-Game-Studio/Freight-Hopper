using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float radius = 1;
    [SerializeField] private float centerForce = 5; // Note, this will be cubed and decrease with radius
    [SerializeField] private LayerMask targetLayer = default;
    [SerializeField] private float explosionOffsetTime = 0.6f;
    [SerializeField] private float explosionDuration;
    private VisualEffect explosion;

    private void Awake()
    {
        explosion = GetComponent<VisualEffect>();
        Explode();
        explosion.Play();
        Invoke(nameof(Explode), explosionOffsetTime);
        Destroy(this.gameObject, explosionDuration);
    }

    private void Explode()
    {
        Vector3 center = this.transform.position;
        Collider[] hitColliders = Physics.OverlapSphere(center, radius, targetLayer);
        foreach (Collider hitObject in hitColliders)
        {
            Rigidbody rb = hitObject.GetComponent<Rigidbody>();
            Debug.Log(rb.name);
            if (rb != null)
            {
                Vector3 position = hitObject.transform.position;
                //float distance = Vector3.Distance(center, position);
                //Vector3 forceDirection = Vector3.Normalize(center - position);
                //float forceAtDistance = (radius - distance) / radius * centerForce;
                //rb.AddForce(forceDirection * forceAtDistance * forceAtDistance * forceAtDistance, ForceMode.Impulse);
                rb.AddExplosionForce(centerForce, center, radius, 3);
            }
        }
    }
}