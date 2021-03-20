using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZone : GravitySource
{
    [SerializeField] private float gravity = 25f;

    [SerializeField] private Quaternion gravityDirection;
    [SerializeField] private Vector3 zone = Vector3.one;
    [SerializeField] private Vector3 falloffZone = Vector3.one * 2;

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
        DrawGizmosArrow(transform.position, gravityDirection * -transform.up);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, 2 * zone);
        if (falloffZone != zone)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(Vector3.zero, 2 * falloffZone);
        }
    }

    private void Awake()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        zone = Vector3.Max(zone, Vector3.zero);
        falloffZone = Vector3.Max(falloffZone, zone);
    }

    // This is very close to being done, but when you touch the edges of the box you get shot straight up for some reason
    public override Vector3 GetGravity(Vector3 position)
    {
        position -= transform.position;
        Vector3 falloffDistances = falloffZone - position.Abs();
        //Debug.Log("Falloff Distances: " + falloffDistances);
        if (falloffDistances.x < 0 || falloffDistances.y < 0 || falloffDistances.z < 0)
        {
            //Debug.Log("no gravity");
            return Vector3.zero;
        }
        Vector3 g = gravity * (gravityDirection * -transform.up).normalized;
        //Vector3 distances = (transform.position + zone) - position.Abs();
        //Debug.Log("Distances: " + distances);
        // This is definitly not right
        /*if (distances.x < 0 || distances.y < 0 || distances.z < 0)
        {
            g *= 1 / (distances - falloffDistances).magnitude;
        }*/
        //Debug.Log("Gravity applied: " + g);
        return g;
    }
}