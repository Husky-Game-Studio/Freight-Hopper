using UnityEngine;

public abstract class GravitySource : MonoBehaviour
{
    public abstract Vector3 GetGravity(Vector3 position);

    private void OnEnable()
    {
        CustomGravity.Register(this);
    }

    private void OnDisable()
    {
        CustomGravity.Unregister(this);
    }
}