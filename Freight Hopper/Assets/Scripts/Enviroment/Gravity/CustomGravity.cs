using System.Collections.Generic;
using UnityEngine;

public static class CustomGravity
{
    private static HashSet<GravitySource> sources = new HashSet<GravitySource>();

    public static Vector3 GetGravity(Vector3 position)
    {
        Vector3 gravity = Vector3.zero;
        foreach (GravitySource source in sources)
        {
            gravity += source.GetGravity(position);
        }
        return gravity;
    }

    public static Vector3 GetGravity(Vector3 position, out Vector3 upAxis)
    {
        Vector3 gravity = Vector3.zero;
        foreach (GravitySource source in sources)
        {
            gravity += source.GetGravity(position);
        }
        upAxis = -gravity.normalized;
        return gravity;
    }

    public static Vector3 GetUpAxis(Vector3 position)
    {
        Vector3 gravity = Vector3.zero;
        foreach (GravitySource source in sources)
        {
            gravity += source.GetGravity(position);
        }
        return -gravity.normalized;
    }

    public static void Register(GravitySource source)
    {
        Debug.Assert(!sources.Contains(source), "Duplicate registeration of gravity source", source);
        sources.Add(source);
    }

    public static void Unregister(GravitySource source)
    {
        Debug.Assert(sources.Contains(source), "Unknown unregisteration of gravity source", source);
        sources.Remove(source);
    }
}