using UnityEngine;

public abstract class GravitySource : MonoBehaviour
{
    [SerializeField] protected float gravity = 25f;

    // It is assumed priority is only changed before runtime. If priority is changed at runtime, CustomGravity needs to reinitialize
    [SerializeField] protected int priority = 0;

    public int GetPriority => priority;

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