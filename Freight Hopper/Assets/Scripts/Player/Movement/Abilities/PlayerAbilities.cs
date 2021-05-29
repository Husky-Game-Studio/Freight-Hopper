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

    private Dictionary<Name, AbilityBehavior> behavior = new Dictionary<Name, AbilityBehavior>();

    private List<AbilityBehavior> abilities = new List<AbilityBehavior>();
    private Rigidbody playerRb;
    private CollisionManagement playerCM;
    private SoundManager playerSM;

    private void OnValidate()
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

        behavior.Clear();
        behavior.Add((Name)Enum.Parse(typeof(Name), nameof(MovementBehavior)), movementBehavior);
        behavior.Add((Name)Enum.Parse(typeof(Name), nameof(JumpBehavior)), jumpBehavior);
        behavior.Add((Name)Enum.Parse(typeof(Name), nameof(GroundPoundBehavior)), groundPoundBehavior);
        behavior.Add((Name)Enum.Parse(typeof(Name), nameof(GrapplePoleBehavior)), grapplePoleBehavior);
        behavior.Add((Name)Enum.Parse(typeof(Name), nameof(DoubleJumpBehavior)), doubleJumpBehavior);
        behavior.Add((Name)Enum.Parse(typeof(Name), nameof(WallRunBehavior)), wallRunBehavior);
        behavior.Add((Name)Enum.Parse(typeof(Name), nameof(UpwardDashBehavior)), upwardDashBehavior);
        behavior.Add((Name)Enum.Parse(typeof(Name), nameof(BurstBehavior)), burstBehavior);
        behavior.Add((Name)Enum.Parse(typeof(Name), nameof(FullStopBehavior)), fullstopBehavior);
    }

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerCM = GetComponent<PhysicsManager>().collisionManager;
        playerSM = GetComponentInChildren<SoundManager>();

        foreach (AbilityBehavior ability in abilities)
        {
            if (ability == null)
            {
                Debug.LogError("Ability script links not found");
            }
            ability.Initialize(playerRb, playerCM, playerSM);
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

    // Unlocks/locks abilities according to current level settings
    public void SetActiveAbilities(Name[] abilitiesNames)
    {
        foreach (AbilityBehavior ability in abilities)
        {
            ability.Lock();
        }
        foreach (Name name in abilitiesNames)
        {
            behavior[name].Unlock();
        }
    }
}