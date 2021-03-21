using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySphere : GravitySource
{
    [SerializeField] private float gravity = 25f;
    [SerializeField, Min(0)] private float radius = 10;
    [SerializeField, Min(0)] private float falloffRadius = 15;

    private float falloffFactor;

    private void OnDrawGizmosSelected()
    {
        Vector3 position = transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, radius);
        if (falloffRadius > radius)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(position, falloffRadius);
        }
    }

    private void Awake()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        falloffRadius = Mathf.Max(falloffRadius, radius);

        falloffFactor = 1 / (falloffRadius - radius);
    }

    /// <summary>
    /// Anything in sphere will have gravity applied on it
    /// </summary>
    public override Vector3 GetGravity(Vector3 position)
    {
        Vector3 posDifference = transform.position - position;

        float distance = posDifference.magnitude;
        if (distance > falloffRadius)
        {
            return Vector3.zero;
        }

        float g = gravity / distance;
        if (distance > radius)
        {
            g *= 1 - (distance - radius) * falloffFactor;
        }

        return g * posDifference;
    }
}