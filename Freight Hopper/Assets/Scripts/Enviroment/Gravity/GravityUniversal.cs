using UnityEngine;

public class GravityUniversal : GravitySource
{
    [SerializeField] private float gravity = 25f;

    /// <summary>
    /// Ignores position, just applies gravity with given -transform.up. Great for most levels
    /// </summary>
    public override Vector3 GetGravity(Vector3 position)
    {
        return -transform.up * gravity;
    }
}