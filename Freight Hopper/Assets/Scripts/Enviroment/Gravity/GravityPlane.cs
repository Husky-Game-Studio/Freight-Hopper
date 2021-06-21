using UnityEngine;

public class GravityPlane : GravitySource
{
    [SerializeField] private Quaternion gravityDirection;
    [SerializeField] private Vector3 centerOffset = Vector3.zero;
    [SerializeField] private Optional<float> falloffRange = new Optional<float>(1);

#if UNITY_EDITOR

    // Draws squares representing an infinite plane. Anything below this is applied for the gravity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        GizmosExtensions.DrawGizmosArrow(this.transform.position + centerOffset, (gravityDirection * -this.transform.up).normalized);

        Vector3 scale = this.transform.localScale;
        scale.y = 1;
        Gizmos.matrix = Matrix4x4.TRS(this.transform.position, this.transform.rotation, scale);
        Vector3 size = new Vector3(2, 0, 2);
        Gizmos.DrawWireCube(centerOffset, size);
        if (falloffRange.Enabled && falloffRange.value > 0)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube((Vector3.up * falloffRange.value) + centerOffset, size);
        }
    }

#endif

    /// <summary>
    /// Like a plane, spans infinitely despite what gizmos shows
    /// Anything below below will have gravity applied on it
    /// </summary>
    public override Vector3 GetGravity(Vector3 position)
    {
        Vector3 up = (gravityDirection * this.transform.up).normalized;
        float distance = Vector3.Dot(up, position - (this.transform.position + centerOffset));
        if (distance > falloffRange.value)
        {
            return Vector3.zero;
        }

        float falloffGravity = -gravity;
        if (falloffRange.Enabled && distance > 0 && falloffRange.value > 0)
        {
            falloffGravity *= 1 - (distance / falloffRange.value);
        }

        return falloffGravity * up;
    }
}