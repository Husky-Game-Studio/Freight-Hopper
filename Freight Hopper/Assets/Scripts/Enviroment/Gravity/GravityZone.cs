using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZone : GravitySource
{
    [SerializeField] private Quaternion gravityDirection;
    [SerializeField] private Vector3 centerOffset = Vector3.zero;
    [SerializeField] private Vector3 zone = Vector3.one;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        GizmosExtensions.DrawGizmosArrow(this.transform.position + centerOffset, gravityDirection * -this.transform.up);
        Gizmos.matrix = Matrix4x4.TRS(this.transform.position, this.transform.rotation, this.transform.localScale);

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

    /// <summary>
    /// Anything in box has gravity applied on it with given direction
    /// </summary>
    public override Vector3 GetGravity(Vector3 position)
    {
        if (!IsPointInBoxRegion(this.transform, centerOffset, zone, position))
        {
            return Vector3.zero;
        }

        return gravity * (gravityDirection * -this.transform.up).normalized;
    }

    /// <summary>
    /// Source: https://math.stackexchange.com/questions/1472049/check-if-a-point-is-inside-a-rectangular-shaped-area-3d
    /// Function for checking if a point is in a box region, true if point is inside the region. Transform is for converting world position to local
    /// probably shouldn't be in gravityzone, but idk where else to put it
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