using UnityEngine;

public abstract class GravitySource : MonoBehaviour
{
    /// <summary>
    /// Using the position of the object, it calculates the gravity for a particular gravity source
    /// Ran every fixed update for every object, may be not good for performance with a large amount of sources
    /// and objects
    /// </summary>
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