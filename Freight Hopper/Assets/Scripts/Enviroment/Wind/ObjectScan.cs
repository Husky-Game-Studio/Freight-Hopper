using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScan
{
    private Collider[] targetColliders;
    private Func<Vector3, bool> checkInsideWindFunction;
    private float rayWidth;
    private float depth;
    private Transform windTransform;
    private Dictionary<Vector3, Ray> viableRays = new Dictionary<Vector3, Ray>();

    public Dictionary<Vector3, Ray> ViableRays => viableRays;

    public ObjectScan(Collider[] colliders, float rayWidth, Transform transform, float depth, Func<Vector3, bool> checkInsideWind)
    {
        targetColliders = colliders;
        this.rayWidth = rayWidth;
        windTransform = transform;
        this.depth = depth;
        checkInsideWindFunction = checkInsideWind;
    }

    public void ShowRays()
    {
        foreach (Ray ray in viableRays.Values)
        {
            Debug.DrawRay(ray.origin, ray.direction * depth, Color.blue);
        }
    }

    public void CreateScan(Ray origin)
    {
        viableRays.Clear();
        AddRays(origin);
    }

    private void AddRays(Ray rayOrigin)
    {
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
        viableRays.Add(w.origin, w);

        while (origins.Count > 0)
        {
            w = origins.Dequeue();

            for (int i = 0; i < directions.Length; i++)
            {
                Ray ray = new Ray(w.origin + (directions[i] * rayWidth), rayOrigin.direction);
                //Debug.DrawRay(ray.origin, ray.direction * 20, Color.red);
                if (viableRays.ContainsKey(ray.origin) || !checkInsideWindFunction(ray.origin + (ray.direction / 100)))
                {
                    continue;
                }

                RaycastHit[] hits = new RaycastHit[targetColliders.Length];
                for (int j = 0; j < targetColliders.Length; j++)
                {
                    targetColliders[j].Raycast(ray, out hits[j], depth);
                }

                bool hitCollider = false;
                for (int j = 0; j < hits.Length; j++)
                {
                    foreach (Collider collider in targetColliders)
                    {
                        if (hits[j].collider == collider)
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

                if (!hitCollider)
                {
                    continue;
                }

                origins.Enqueue(ray);
                viableRays.Add(ray.origin, ray);
            }
        }
    }
}