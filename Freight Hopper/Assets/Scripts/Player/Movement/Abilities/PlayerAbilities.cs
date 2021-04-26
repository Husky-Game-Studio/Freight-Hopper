using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionManagement), typeof(Rigidbody))]
public class PlayerAbilities : MonoBehaviour
{
    public MovementBehavior movementBehavior;
    public JumpBehavior jumpBehavior;
    public GroundPoundBehavior groundPoundBehavior;
    public GrapplePoleBehavior grapplePoleBehavior;
    public DoubleJumpBehavior doubleJumpBehavior;
    public WallRunBehavior wallRunBehavior;
    public UpwardDashBehavior upwardDashBehavior;
    public BurstBehavior burstBehavior;
    public FullStopBehavior fullstopBehavior;

    private List<AbilityBehavior> abilities = new List<AbilityBehavior>();
    public List<AbilityBehavior> Abilities => abilities;
    private Rigidbody playerRb;
    private CollisionManagement playerCM;

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
    }

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerCM = GetComponent<CollisionManagement>();

        foreach (AbilityBehavior ability in abilities)
        {
            if (ability == null)
            {
                Debug.LogError("Ability script links not found");
            }
            ability.LinkPhysicsInformation(playerRb, playerCM);
        }
        doubleJumpBehavior.GetJumpBehavior(jumpBehavior);
    }
}