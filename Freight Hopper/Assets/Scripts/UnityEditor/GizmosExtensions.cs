using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

public static class GizmosExtensions
{
    /// <summary>
    /// Surprising, it draws an arrow using gizmos
    /// </summary>
    public static void DrawGizmosArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.DrawRay(pos, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }

    /// <summary>
    /// Surprising, it draws an arrow using gizmos
    /// </summary>
    public static void DrawGizmosArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.color = color;
        Gizmos.DrawRay(pos, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }

    /// <summary>
    /// Draws a capsule, from https://forum.unity.com/threads/drawing-capsule-gizmo.354634/
    /// </summary>
    public static void DrawWireCapsule(Vector3 pos, Vector3 pos2, float radius, Color color = default)
    {
        if (color != default) Handles.color = color;

        var forward = pos2 - pos;
        var rot = Quaternion.LookRotation(forward);
        var pointOffset = radius / 2f;
        var length = forward.magnitude;
        var center2 = new Vector3(0f, 0, length);

        Matrix4x4 angleMatrix = Matrix4x4.TRS(pos, rot, Handles.matrix.lossyScale);

        using (new Handles.DrawingScope(angleMatrix))
        {
            Handles.DrawWireDisc(Vector3.zero, Vector3.forward, radius);
            Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.left * pointOffset, -180f, radius);
            Handles.DrawWireArc(Vector3.zero, Vector3.left, Vector3.down * pointOffset, -180f, radius);
            Handles.DrawWireDisc(center2, Vector3.forward, radius);
            Handles.DrawWireArc(center2, Vector3.up, Vector3.right * pointOffset, -180f, radius);
            Handles.DrawWireArc(center2, Vector3.left, Vector3.up * pointOffset, -180f, radius);

            DrawLine(radius, 0f, length);
            DrawLine(-radius, 0f, length);
            DrawLine(0f, radius, length);
            DrawLine(0f, -radius, length);
        }
    }

    /// <summary>
    /// Helper for capsule wire drawing
    /// </summary>
    private static void DrawLine(float arg1, float arg2, float forward)
    {
        Handles.DrawLine(new Vector3(arg1, arg2, 0f), new Vector3(arg1, arg2, forward));
    }

    // Scales gizmos based off camera and its world position
    public static float GetGizmoSize(Vector3 position)
    {
        Camera current = Camera.current;
        position = Gizmos.matrix.MultiplyPoint(position);

        if (current)
        {
            Transform transform = current.transform;
            Vector3 position2 = transform.position;
            float z = Vector3.Dot(position - position2, transform.TransformDirection(new Vector3(0f, 0f, 1f)));
            Vector3 a = current.WorldToScreenPoint(position2 + transform.TransformDirection(new Vector3(0f, 0f, z)));
            Vector3 b = current.WorldToScreenPoint(position2 + transform.TransformDirection(new Vector3(1f, 0f, z)));
            float magnitude = (a - b).magnitude;
            return 80f / Mathf.Max(magnitude, 0.0001f);
        }

        return 20f;
    }
}

#endif