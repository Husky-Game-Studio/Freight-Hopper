using UnityEngine;

[System.Serializable]
public class AbilityBehavior : MonoBehaviour
{
    [SerializeField, ReadOnly] protected bool unlocked = false;
    [SerializeField, ReadOnly] protected bool consumed = false;
    [SerializeField, ReadOnly] protected bool preventConsumption = false;
    public bool Unlocked => unlocked;
    public bool Consumed => consumed;

    // Ready meaning not consumed
    public bool UnlockedAndReady => this.Unlocked && !this.Consumed;

    protected Rigidbody rb;
    protected SoundManager soundManager;
    protected PlayerAbilities abilitiesManager;

    public virtual void Initialize(Rigidbody rb, SoundManager sm, PlayerAbilities pa)
    {
        this.rb = rb;
        this.soundManager = sm;
        this.abilitiesManager = pa;
    }

    public SoundManager PlayerSoundManager() => soundManager;

    public virtual void EntryAction()
    {
    }

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

    // Action played while in Ability state
    public virtual void Action() { }

    public virtual void Recharge()
    {
        consumed = false;
    }

    public void PreventConsumption()
    {
        preventConsumption = true;
    }

    public void Lock()
    {
        unlocked = false;
    }

    public void Unlock()
    {
        unlocked = true;
    }
}