using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZone : GravitySource
{
    [SerializeField] private float gravity = 25f;
    [SerializeField] private Quaternion gravityDirection;
    [SerializeField] private Vector3 centerOffset = Vector3.zero;
    [SerializeField] private Vector3 zone = Vector3.one;

    public static void DrawGizmosArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.DrawRay(pos, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }

    public static void DrawGizmosArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.color = color;
        Gizmos.DrawRay(pos, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        DrawGizmosArrow(transform.position + centerOffset, gravityDirection * -transform.up);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        Gizmos.DrawWireCube(centerOffset, 2 * zone);
    }

    private void Awake()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        zone = Vector3.Max(zone, Vector3.zero);
    }

    public override Vector3 GetGravity(Vector3 position)
    {
        if (!IsPointInBoxRegion(this.transform, centerOffset, zone, position))
        {
            return Vector3.zero;
        }

        return gravity * (gravityDirection * -transform.up).normalized;
    }

    /// <summary>
    /// Source: https://math.stackexchange.com/questions/1472049/check-if-a-point-is-inside-a-rectangular-shaped-area-3d
    /// Function for checking if a point is in a box region, true if point is inside the region. Transform is for converting world position to local
    /// </summary>
    public static bool IsPointInBoxRegion(Transform transform, Vector3 center, Vector3 bounds, Vector3 point)
    {
        Vector3 p1 = transform.TransformPoint(center - bounds);
        Vector3 p2 = transform.TransformPoint(new Vector3(center.x - bounds.x, center.y - bounds.y, center.z + bounds.z));
        Vector3 p3 = transform.TransformPoint(new Vector3(center.x + bounds.x, center.y - bounds.y, center.z - bounds.z));
        Vector3 p4 = transform.TransformPoint(new Vector3(center.x - bounds.x, center.y + bounds.y, center.z - bounds.z));

        Vector3 U = p1 - p2;
        Vector3 V = p1 - p3;
        Vector3 W = p1 - p4;

        bool caseOne = Vector3.Dot(U, p2) < Vector3.Dot(U, point) && Vector3.Dot(U, point) < Vector3.Dot(U, p1);
        bool caseTwo = Vector3.Dot(V, p3) < Vector3.Dot(V, point) && Vector3.Dot(V, point) < Vector3.Dot(V, p1);
        bool caseThree = Vector3.Dot(W, p4) < Vector3.Dot(W, point) && Vector3.Dot(W, point) < Vector3.Dot(W, p1);

        return caseOne && caseTwo && caseThree;
    }
}