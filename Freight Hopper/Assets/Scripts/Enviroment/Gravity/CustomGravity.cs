using System.Collections.Generic;
using UnityEngine;

public static class CustomGravity
{
    private static HashSet<GravitySource> sources = new HashSet<GravitySource>();
    private static Vector3 gravityCache;
    private static bool gravityCachesFilled = false;
    private static Vector3 upAxisCache;

    /// <summary>
    /// O(N), cycles through gravity sources to get gravity of position
    /// </summary>
    public static Vector3 GetGravity(Vector3 position)
    {
        if (gravityCachesFilled)
        {
            return gravityCache;
        }

        Vector3 gravity = Vector3.zero;
        int highestPriority = int.MinValue;
        foreach (GravitySource source in sources)
        {
            Vector3 sourceGravity = source.GetGravity(position);
            if (sourceGravity == Vector3.zero)
            {
                continue;
            }
            if (source.GetPriority > highestPriority)
            {
                gravity = Vector3.zero;
                highestPriority = source.GetPriority;
            }
            if (source.GetPriority == highestPriority)
            {
                gravity += source.GetGravity(position);
            }
        }
        return gravity;
    }

    /// <summary>
    /// O(N), cycles through gravity sources to get gravity and UpAxis.
    /// Use this function if you are doing both its better on performance
    /// </summary>
    public static Vector3 GetGravity(Vector3 position, out Vector3 upAxis)
    {
        Vector3 gravity = GetGravity(position);
        if (gravityCachesFilled)
        {
            upAxis = upAxisCache;
        }
        else
        {
            upAxis = -gravity.normalized;
        }

        return gravity;
    }

    /// <summary>
    /// O(N), Cycles through all gravity sources to calculate current position's upAxis
    /// </summary>
    public static Vector3 GetUpAxis(Vector3 position)
    {
        if (gravityCachesFilled)
        {
            return upAxisCache;
        }
        return -GetGravity(position).normalized;
    }

    /// <summary>
    /// Registers source to be considered for calculations
    /// </summary>
    public static void Register(GravitySource source)
    {
        Debug.Assert(!sources.Contains(source), "Duplicate registeration of gravity source", source);
        sources.Add(source);
        if (sources.Count == 1)
        {
            if (source is GravityUniversal)
            {
                gravityCache = source.GetGravity(Vector3.zero);
                upAxisCache = -gravityCache.normalized;
                gravityCachesFilled = true;
            }
            else
            {
                gravityCachesFilled = false;
            }
        }
        else
        {
            gravityCachesFilled = false;
        }
    }

    /// <summary>
    /// Unregisters source, good for if performance is an issue. But then source won't be considered for gravity calculations
    /// </summary>
    public static void Unregister(GravitySource source)
    {
        Debug.Assert(sources.Contains(source), "Unknown unregisteration of gravity source", source);
        sources.Remove(source);
        gravityCachesFilled = false;
    }
}