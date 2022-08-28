using UnityEngine;

public static class CustomGravity
{
    public static Vector3 GetGravity() => Physics.gravity;
    public static Vector3 GetUpAxis() => Vector3.up;

    public static Vector3 GetGravity(out Vector3 upAxis)
    {
        upAxis = Vector3.up;
        return GetGravity();
    }
}