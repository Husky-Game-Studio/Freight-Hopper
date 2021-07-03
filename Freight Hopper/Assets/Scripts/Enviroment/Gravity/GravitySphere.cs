using UnityEngine;

public class GravitySphere : GravitySource
{
    [SerializeField, Min(0)] private float radius = 10;
    [SerializeField] private Optional<float> falloffRadius = new Optional<float>(15);

    private float falloffFactor;

    // Generates sphere gizmos. One for radius and one for falloff
    private void OnDrawGizmosSelected()
    {
        Vector3 position = this.transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, radius);
        if (falloffRadius.Enabled && falloffRadius.value > radius)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(position, falloffRadius.value);
        }
    }

    private void Awake()
    {
        if (falloffRadius.Enabled)
        {
            falloffRadius.value = Mathf.Max(falloffRadius.value, radius);

            falloffFactor = 1 / (falloffRadius.value - radius);
        }
        else
        {
            falloffRadius.value = radius;
        }
    }

    private void OnValidate()
    {
        if (falloffRadius.Enabled)
        {
            falloffRadius.value = Mathf.Max(falloffRadius.value, radius);

            falloffFactor = 1 / (falloffRadius.value - radius);
        }
        else
        {
            falloffRadius.value = radius;
        }
    }

    /// <summary>
    /// Anything in sphere will have gravity applied on it
    /// </summary>
    public override Vector3 GetGravity(Vector3 position)
    {
        Vector3 posDifference = this.transform.position - position;

        float distance = posDifference.magnitude;
        if (distance > falloffRadius.value)
        {
            return Vector3.zero;
        }

        // This shouldn't happen, but if it does at least the game is still running
        if (distance == 0)
        {
            Debug.LogError("Distance from gravity sphere is 0 causing divide by 0 excpetion");
            Debug.Log("The position of the sphere is " + this.transform.position + " and the position is " + position);
            return gravity * posDifference;
        }

        float falloffGravity = gravity / distance;

        if (falloffRadius.Enabled && distance > radius)
        {
            falloffGravity *= 1 - ((distance - radius) * falloffFactor);
        }

        return falloffGravity * posDifference;
    }
}