using UnityEngine;

public class GravityUniversal : GravitySource
{
    [SerializeField] private bool dynamic = false;
    private Vector3 up;

    private void Awake()
    {
        up = -this.transform.up;
    }

    /// <summary>
    /// Ignores position, just applies gravity with given -transform.up. Great for most levels
    /// </summary>
    public override Vector3 GetGravity(Vector3 position)
    {
        if (dynamic)
        {
            up = -this.transform.up;
        }

        return up * gravity;
    }
}