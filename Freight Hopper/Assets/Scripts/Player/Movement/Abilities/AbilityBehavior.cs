using UnityEngine;

[System.Serializable]
public class AbilityBehavior : MonoBehaviour
{
    [SerializeField, ReadOnly] protected bool consumed = false;
    [SerializeField, ReadOnly] protected bool preventConsumption = false;
    public bool Consumed => consumed;

    [SerializeField, ReadOnly] protected Rigidbody rb;
    protected SoundManager soundManager;

    public virtual void Initialize()
    {
        this.rb = Player.Instance.modules.rigidbody;
        this.soundManager = Player.Instance.modules.soundManager;
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
    public virtual void Action()
    { }

    public virtual void Recharge()
    {
        consumed = false;
    }

    public void PreventConsumption()
    {
        preventConsumption = true;
    }
}