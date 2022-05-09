using System.Collections.Generic;
using UnityEngine;

public static class CustomGravity
{
    public static Vector3 GetGravity()
    {
        return Vector3.up * -35;
    }

    public static Vector3 GetGravity(out Vector3 upAxis)
    {
        Vector3 gravity = GetGravity();
        upAxis = Vector3.up;
        return gravity;
    }

    public static Vector3 GetUpAxis()
    {
        return Vector3.up;
    }
}