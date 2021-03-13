using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySphere : GravitySource
{
    [SerializeField] private float gravity = 25f;
    [SerializeField, Min(0)] private float outerRadius = 10;
    [SerializeField, Min(0)] private float outerFalloffRadius = 15;
    [SerializeField, Min(0)] private float innerRadius = 5;
    [SerializeField, Min(0)] private float innerFalloffRadius = 1;
    private float outerFalloffFactor;
    private float innerFalloffFactor;

    private void OnDrawGizmosSelected()
    {
        Vector3 position = transform.position;
        if (innerFalloffRadius > 0 && innerFalloffRadius < innerRadius)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(position, innerFalloffRadius);
        }
        Gizmos.color = Color.yellow;
        if (innerRadius > 0 && innerRadius < outerRadius)
        {
            Gizmos.DrawWireSphere(position, innerRadius);
        }
        Gizmos.DrawWireSphere(position, outerRadius);
        if (outerFalloffRadius > outerRadius)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(position, outerFalloffRadius);
        }
    }

    private void Awake()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        innerFalloffRadius = Mathf.Max(innerFalloffRadius, 0);
        innerRadius = Mathf.Max(innerRadius, innerFalloffRadius);
        outerRadius = Mathf.Max(outerRadius, innerRadius);
        outerFalloffRadius = Mathf.Max(outerFalloffRadius, outerRadius);

        innerFalloffFactor = 1 / (innerRadius - innerFalloffRadius);
        outerFalloffFactor = 1 / (outerFalloffRadius - outerRadius);
    }

    public override Vector3 GetGravity(Vector3 position)
    {
        Vector3 posDifference = transform.position - position;
        float distance = posDifference.magnitude;
        if (distance > outerFalloffRadius || distance < innerFalloffRadius)
        {
            return Vector3.zero;
        }
        float g = gravity / distance;
        if (distance > outerRadius)
        {
            g *= 1 - (distance - outerRadius) * outerFalloffFactor;
        }
        else if (distance < innerRadius)
        {
            g *= 1 - (innerRadius - distance) * innerFalloffFactor;
        }
        return g * posDifference;
    }
}