using UnityEngine;

public class GravityPlane : GravitySource
{
    [SerializeField] private float gravity = 25f;
    [SerializeField] private Quaternion gravityDirection;
    [SerializeField] private Vector3 centerOffset = Vector3.zero;
    [SerializeField, Min(0)] private float falloffRange = 1;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        GizmosExtensions.DrawGizmosArrow(transform.position + centerOffset, (gravityDirection * -transform.up).normalized);

        Vector3 scale = transform.localScale;
        scale.y = 1;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, scale);
        Vector3 size = new Vector3(2, 0, 2);
        Gizmos.DrawWireCube(centerOffset, size);
        if (falloffRange > 0)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube((Vector3.up * falloffRange) + centerOffset, size);
        }
    }

    /// <summary>
    /// Like a plane, spans infinitely despite what gizmos shows
    /// Anything below below will have gravity applied on it
    /// </summary>
    public override Vector3 GetGravity(Vector3 position)
    {
        Vector3 up = (gravityDirection * transform.up).normalized;
        float distance = Vector3.Dot(up, position - (transform.position + centerOffset));
        if (distance > falloffRange)
        {
            return Vector3.zero;
        }

        float falloffGravity = -gravity;
        if (distance > 0)
        {
            falloffGravity *= 1 - distance / falloffRange;
        }

        return falloffGravity * up;
    }
}