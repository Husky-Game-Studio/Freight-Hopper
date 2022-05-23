using UnityEngine;
using UnityEngine.InputSystem;

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
    
    public void PreventConsumptionCheck()
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

    public virtual void Recharge()
    {
        consumed = false;
    }

    public void PreventConsumption()
    {
        preventConsumption = true;
    }
}