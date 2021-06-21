using UnityEngine;

public class GravityUniversal : GravitySource
{
    /// <summary>
    /// Ignores position, just applies gravity with given -transform.up. Great for most levels
    /// </summary>
    public override Vector3 GetGravity(Vector3 position)
    {
        return -this.transform.up * gravity;
    }
}