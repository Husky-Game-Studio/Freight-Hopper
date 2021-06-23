using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(PhysicsManager), typeof(Rigidbody))]
public class PlayerAbilities : MonoBehaviour
{
    public MovementBehavior movementBehavior;
    public JumpBehavior jumpBehavior;
    public WallRunBehavior wallRunBehavior;
    public GroundPoundBehavior groundPoundBehavior;
    public GrapplePoleBehavior grapplePoleBehavior;
    public DoubleJumpBehavior doubleJumpBehavior;
    public UpwardDashBehavior upwardDashBehavior;
    public BurstBehavior burstBehavior;
    public FullStopBehavior fullstopBehavior;

    public enum Name
    {
        MovementBehavior,
        JumpBehavior,
        WallRunBehavior,
        GroundPoundBehavior,
        GrapplePoleBehavior,
        DoubleJumpBehavior,
        UpwardDashBehavior,
        BurstBehavior,
        FullStopBehavior
    }

    private List<Name> abilityNames = new List<Name>() {
        (Name)Enum.Parse(typeof(Name), nameof(MovementBehavior)),
        (Name)Enum.Parse(typeof(Name), nameof(JumpBehavior)),
        (Name)Enum.Parse(typeof(Name), nameof(GroundPoundBehavior)),
        (Name)Enum.Parse(typeof(Name), nameof(GrapplePoleBehavior)),
        (Name)Enum.Parse(typeof(Name), nameof(DoubleJumpBehavior)),
        (Name)Enum.Parse(typeof(Name), nameof(WallRunBehavior)),
        (Name)Enum.Parse(typeof(Name), nameof(UpwardDashBehavior)),
        (Name)Enum.Parse(typeof(Name), nameof(BurstBehavior)),
        (Name)Enum.Parse(typeof(Name), nameof(FullStopBehavior))
    };

    [SerializeField, HideInInspector] private List<AbilityBehavior> abilities = new List<AbilityBehavior>();
    [SerializeField, HideInInspector] private PhysicsManager playerPM;
    [SerializeField, HideInInspector] private SoundManager playerSM;

    private void Awake()
    {
        movementBehavior = GetComponentInChildren<MovementBehavior>();
        jumpBehavior = GetComponentInChildren<JumpBehavior>();
        groundPoundBehavior = GetComponentInChildren<GroundPoundBehavior>();
        grapplePoleBehavior = GetComponentInChildren<GrapplePoleBehavior>();
        doubleJumpBehavior = GetComponentInChildren<DoubleJumpBehavior>();
        wallRunBehavior = GetComponentInChildren<WallRunBehavior>();
        upwardDashBehavior = GetComponentInChildren<UpwardDashBehavior>();
        burstBehavior = GetComponentInChildren<BurstBehavior>();
        fullstopBehavior = GetComponentInChildren<FullStopBehavior>();

        playerPM = GetComponent<PhysicsManager>();
        playerSM = GetComponentInChildren<SoundManager>();
        foreach (AbilityBehavior ability in abilities)
        {
            if (ability == null)
            {
                Debug.LogError("Ability script links not found");
            }
            ability.Initialize(playerPM, playerSM);
        }
        doubleJumpBehavior.GetJumpBehavior(jumpBehavior);
    }

    // Recharges all abilities. Meaning they can be used again
    public void Recharge()
    {
        foreach (AbilityBehavior ability in abilities)
        {
            ability.Recharge();
        }
    }

    // Unlocks/locks abilities according to current level settings. Bad optimization
    public void SetActiveAbilities(Name[] abilitiesNames)
    {
        abilities.Clear();
        abilities.Add(movementBehavior);
        abilities.Add(jumpBehavior);
        abilities.Add(groundPoundBehavior);
        abilities.Add(grapplePoleBehavior);
        abilities.Add(doubleJumpBehavior);
        abilities.Add(wallRunBehavior);
        abilities.Add(upwardDashBehavior);
        abilities.Add(burstBehavior);
        abilities.Add(fullstopBehavior);

        foreach (AbilityBehavior ability in abilities)
        {
            ability.Lock();
        }
        for (int i = 0; i < abilities.Count; i++)
        {
            foreach (Name name in abilitiesNames)
            {
                if (abilityNames[i] == name)
                {
                    abilities[i].Unlock();
                    break;
                }
            }
        }
    }
}