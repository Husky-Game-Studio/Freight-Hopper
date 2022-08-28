using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 Abs(this Vector3 vector)
    {
        return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
    }
    
    // Basically rotates a vector onto the contact plane. Make sure to use the CollisionDataCollected event when using this
    public static Vector3 ProjectOnContactPlane(this Vector3 vector, Vector3 normal)
    {
        return vector - (normal * Vector3.Dot(vector, normal));
    }

    public static bool IsZero(this Vector3 vector)
    {
        return vector == Vector3.zero;
    }

    public static Vector3 ClampComponents(this Vector3 vector, Vector3 minBounds, Vector3 maxBounds)
    {
        return new Vector3(
            Mathf.Clamp(vector.x, minBounds.x, maxBounds.x),
            Mathf.Clamp(vector.y, minBounds.y, maxBounds.y),
            Mathf.Clamp(vector.z, minBounds.z, maxBounds.z));
    }
}