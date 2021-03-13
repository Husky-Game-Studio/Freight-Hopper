using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPlane : GravitySource
{
    [SerializeField] private float gravity = 9.81f;
    [SerializeField, Min(0)] private float range = 1;

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 size = new Vector3(1, 0, 1);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(1, 0, 1));
        if (range > 0)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(Vector3.up * range, size);
        }
        Vector3 scale = transform.localScale;
        scale.y = range;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, scale);
    }

    public override Vector3 GetGravity(Vector3 position)
    {
        Vector3 up = transform.up;
        float distance = Vector3.Dot(up, position - transform.position);
        if (distance > range)
        {
            return Vector3.zero;
        }

        float falloffGravity = -gravity;
        if (distance > 0)
        {
            falloffGravity *= 1 - distance / range;
        }

        return falloffGravity * up;
    }
}