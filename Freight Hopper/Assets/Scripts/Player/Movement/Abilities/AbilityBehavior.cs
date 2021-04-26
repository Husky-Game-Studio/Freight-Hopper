using UnityEngine;

[System.Serializable]
public abstract class AbilityBehavior : MonoBehaviour
{
    [SerializeField] protected bool unlocked = true;
    [SerializeField, ReadOnly] protected bool consumed = false;
    [SerializeField, ReadOnly] protected bool preventConsumption = false;
    public bool IsUnlocked => unlocked;
    public bool IsConsumed => consumed;
    protected Rigidbody playerRb;
    protected CollisionManagement playerCM;

    /// <summary>
    /// Assigns the rigidbody and collision management for the ability
    /// </summary>
    public virtual void LinkPhysicsInformation(Rigidbody rb, CollisionManagement cm)
    {
        this.playerRb = rb;
        this.playerCM = cm;
    }

    /// <summary>
    /// Action played when entering state for ability
    /// </summary>
    public abstract void EntryAction();

    /// <summary>
    /// Action player when leaving state for ability
    /// </summary>
    public virtual void ExitAction()
    {
        if (preventConsumption)
        {
            preventConsumption = false;
        }
        else
        {
            consumed = true;
        }
    }

    /// <summary>
    /// Action played while in Ability state
    /// </summary>
    public abstract void Action();

    /// <summary>
    /// Sets consumed to false to allow ability to be used again
    /// </summary>
    public virtual void Recharge()
    {
        consumed = false;
    }

    public void PreventConsumption()
    {
        preventConsumption = true;
    }

    /// <summary>
    /// Sets unlocked to false
    /// </summary>
    public void Lock()
    {
        unlocked = false;
    }

    /// <summary>
    /// Sets unlocked to true
    /// </summary>
    public void Unlock()
    {
        unlocked = true;
    }
}