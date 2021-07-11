using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScan
{
    private Transform targetTransform;
    private Collider[] targetColliders;
    private Func<Vector3, bool> checkInsideWindFunction;
    private float rayWidth;
    private Ray rayOrigin;
    private float depth;
    private Transform windTransform;
    private Dictionary<Vector3, Ray> viableRays = new Dictionary<Vector3, Ray>();

    public ObjectScan(Transform targetTransform, Collider[] colliders, float rayWidth, Ray origin, Transform transform, float depth, Func<Vector3, bool> checkInsideWind)
    {
        this.targetTransform = targetTransform;
        targetColliders = colliders;
        this.rayOrigin = origin;
        this.rayWidth = rayWidth;
        windTransform = transform;
        this.depth = depth;
        checkInsideWindFunction = checkInsideWind;
    }

    public void UpdateOrigin(Ray origin)
    {
        this.rayOrigin = origin;
    }

    public void ShowRays()
    {
        foreach (Ray ray in viableRays.Values)
        {
            Debug.DrawRay(ray.origin, ray.direction * depth, Color.blue);
        }
    }

    public void CreateScan()
    {
        viableRays.Clear();
        AddRay(rayOrigin.origin);
    }

    private void AddRay(Vector3 origin)
    {
        Vector3[] offsets =
        {
            windTransform.transform.up,
            -windTransform.transform.up,
            windTransform.transform.right,
            -windTransform.transform.right
        };
        for (int i = 0; i < offsets.Length; i++)
        {
            Ray ray = new Ray(origin + (offsets[i] * rayWidth), rayOrigin.direction);
            RaycastHit hit;
            targetColliders[0].Raycast(ray, out hit, depth);
            bool hitCollider = false;
            foreach (Collider collider in targetColliders)
            {
                if (hit.collider == collider)
                {
                    hitCollider = true;
                }
            }
            if (!hitCollider)
            {
                continue;
            }
            if (!viableRays.ContainsKey(ray.origin) && checkInsideWindFunction(ray.origin))
            {
                viableRays.Add(ray.origin, ray);
                AddRay(ray.origin);
            }
        }
    }
}