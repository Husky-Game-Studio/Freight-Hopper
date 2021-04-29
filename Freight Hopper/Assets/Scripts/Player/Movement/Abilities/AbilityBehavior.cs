using UnityEngine;

[System.Serializable]
public abstract class AbilityBehavior : MonoBehaviour
{
    [SerializeField, ReadOnly] protected bool unlocked = false;
    [SerializeField, ReadOnly] protected bool consumed = false;
    [SerializeField, ReadOnly] protected bool preventConsumption = false;
    public bool Unlocked => unlocked;
    public bool Consumed => consumed;
    protected Rigidbody playerRb;
    protected CollisionManagement playerCM;
    protected SoundManager playerSM;

    /// <summary>
    /// Links abilities to components they will need to function
    /// </summary>
    public virtual void Initialize(Rigidbody rb, CollisionManagement cm, SoundManager sm)
    {
        this.playerRb = rb;
        this.playerCM = cm;
        this.playerSM = sm;
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