using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScanner
{
    private Collider[] targetColliders;
    private Func<Vector3, bool> checkInsideWindFunction;
    private float rayWidth;
    private float depth;
    private Transform windTransform;
    private Dictionary<Vector3Int, Ray> viableRays;

    public ObjectScanner(Collider[] colliders, float rayWidth, Dictionary<Vector3Int, Ray> viableRays,
        Transform transform, float depth, Func<Vector3, bool> checkInsideWind)
    {
        this.viableRays = viableRays;
        targetColliders = colliders;
        this.rayWidth = rayWidth;
        windTransform = transform;
        this.depth = depth;
        checkInsideWindFunction = checkInsideWind;
    }

    public void AddToScan(Ray origin)
    {
        AddRays(origin);
    }

    public bool IsViableRay(Ray ray)
    {
        if (viableRays.ContainsKey(ConvertVector3Int(ray.origin)) || !checkInsideWindFunction(ray.origin + (ray.direction / 100)))
        {
            return false;
        }

        bool hitCollider = false;
        for (int j = 0; j < targetColliders.Length; j++)
        {
            RaycastHit hit;
            targetColliders[j].Raycast(ray, out hit, depth);
            foreach (Collider collider in targetColliders)
            {
                if (collider == null)
                {
                    continue;
                }
                if (hit.collider == collider)
                {
                    hitCollider = true;
                    break;
                }
            }
            if (hitCollider)
            {
                break;
            }
        }

        return hitCollider;
    }

    private Vector3Int ConvertVector3Int(Vector3 vector3)
    {
        return Vector3Int.RoundToInt(100 * vector3);
    }

    private void AddRays(Ray rayOrigin)
    {
        if (!IsViableRay(rayOrigin))
        {
            return;
        }
        Queue<Ray> origins = new Queue<Ray>();

        Vector3[] directions =
        {
            windTransform.transform.up,
            -windTransform.transform.up,
            windTransform.transform.right,
            -windTransform.transform.right
        };
        Ray w = new Ray(rayOrigin.origin, rayOrigin.direction);
        origins.Enqueue(w);
        viableRays.Add(ConvertVector3Int(w.origin), w);
        while (origins.Count > 0)
        {
            w = origins.Dequeue();

            for (int i = 0; i < directions.Length; i++)
            {
                Ray ray = new Ray(w.origin + (directions[i] * rayWidth), rayOrigin.direction);
                if (!IsViableRay(ray))
                {
                    continue;
                }

                origins.Enqueue(ray);
                viableRays.Add(ConvertVector3Int(ray.origin), ray);
            }
        }
    }
}