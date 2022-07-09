using UnityEngine;

public static class TargetSphereDrive
{
    private static float Radius(float x, float y)
    {
        return (x * x + y * y) / (2 * x);
    }

    private static float Theta(float x, float y)
    {
        return Mathf.Atan2(2 * x * y , y * y - x * x);
    }

    private static float ArcLength(float x, float y)
    {
        return (x == 0) ? y : Radius(x, y) * Theta(x, y);
    }

    public static Vector3 TargetAngularVelocity(Vector3 v)
    {
        float yRot = Theta(v.x, v.z);
        float arcLXZ = ArcLength(v.x, v.z);
        float xRot = -Theta(v.y, arcLXZ);
        float arcLXYZ = ArcLength(v.y, arcLXZ);
        Quaternion targetRot = Quaternion.Euler(Mathf.Rad2Deg * xRot, Mathf.Rad2Deg * yRot, 0); // Consideration for custom gravity: Quaternion.LookRotation(Vector3.forward, Vector3.up)
        Vector3 axis;
        float angle;
        targetRot.ToAngleAxis(out angle, out axis);
        angle -= (angle > 180) ? 360 : 0;
        return Mathf.Deg2Rad * angle * axis.normalized / arcLXYZ;
    }
}
